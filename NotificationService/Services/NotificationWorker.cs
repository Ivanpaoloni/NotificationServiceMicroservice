using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationService.Infrastructure;
using NotificationService.Models;
using NotificationService.Policies;
using NotificationService.Services.Interfaces;
using Polly.Retry;

namespace NotificationService.Services
{
    public class NotificationWorker : BackgroundService
    {
        private readonly INotificationQueue _queue;
        private readonly ILogger<NotificationWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private DateTime _lastDbCheck = DateTime.MinValue;
        private static readonly TimeSpan _dbCheckInterval = TimeSpan.FromSeconds(30); // Intervalo para verificar la base de datos

        public NotificationWorker(INotificationQueue queue, IServiceScopeFactory scopeFactory, ILogger<NotificationWorker> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                NotificationDbContext dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
                INotificationSenderFactory senderFactory = scope.ServiceProvider.GetRequiredService<INotificationSenderFactory>();

                if (DateTime.UtcNow - _lastDbCheck >= _dbCheckInterval)
                {
                    _lastDbCheck = DateTime.UtcNow;

                    var pendingNotifications = await dbContext.NotificationMessages
                        .Where(n => n.Status == NotificationStatusTypeEnum.Pending ||
                                    (n.Status == NotificationStatusTypeEnum.Failed && n.RetryCount < 3))
                        .OrderBy(n => n.CreatedAt)
                        .Take(10) // Limitamos para evitar encolar de más
                        .ToListAsync(stoppingToken);

                    if (pendingNotifications.Count == 0)
                    {

                        _logger.LogInformation("No pending notifications to process");
                        await Task.Delay(1000, stoppingToken); // Esperar un segundo si no hay notificaciones pendientes
                        continue;
                    }

                    // Encolar las notificaciones pendientes
                    _logger.LogInformation("Found {Count} pending notifications to process", pendingNotifications.Count);
                    foreach (var notification in pendingNotifications)
                    {
                        _queue.Enqueue(new NotificationRequest
                        {
                            Id = notification.Id,
                            Channel = notification.Channel,
                            Recipient = notification.Recipient,
                            Subject = notification.Subject,
                            Message = notification.Message
                        });
                    }
                }

                // Procesar la cola de notificaciones
                if (_queue.TryDequeue(out var request))
                {
                    INotificationSender sender = senderFactory.GetSender(request!.Channel);
                    AsyncRetryPolicy retryPolicy = NotificationRetryPolicies.GetDefaultRetryPolicy(_logger);

                    try
                    {
                        await retryPolicy.ExecuteAsync(async () =>
                        {
                            await sender.SendAsync(request.Recipient, request.Subject, request.Message);
                        });
                        NotificationMessage? entity = await dbContext.NotificationMessages.FindAsync(request.Id);
                        if (entity != null)
                        {
                            entity.Status = NotificationStatusTypeEnum.Sent;
                            entity.SentAt = DateTime.UtcNow;
                            await dbContext.SaveChangesAsync();
                        }
                        _logger.LogInformation("Notification sent successfully to {Recipient}", request.Recipient);

                    }
                    catch (Exception ex)
                    {
                        NotificationMessage? entity = await dbContext.NotificationMessages.FindAsync(request.Id);
                        if (entity != null)
                        {
                            entity.Status = NotificationStatusTypeEnum.Failed;
                            entity.RetryCount++;
                            await dbContext.SaveChangesAsync();
                        }
                        _logger.LogError(ex, "Final failure after retries sending notification to {Recipient}", request.Recipient);
                    }
                }
                await Task.Delay(100, stoppingToken); // Pequeño delay para no saturar el CPU
            }
        }
    }
}
