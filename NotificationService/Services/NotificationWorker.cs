using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
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
        private static readonly TimeSpan _dbCheckInterval = TimeSpan.FromSeconds(30);
        private readonly WorkerStatusService _workerStatusService;

        public NotificationWorker(INotificationQueue queue, IServiceScopeFactory scopeFactory, ILogger<NotificationWorker> logger, WorkerStatusService workerStatusService)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _workerStatusService = workerStatusService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // new scope foreach iteration
                using var scope = _scopeFactory.CreateScope();
                NotificationDbContext dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
                INotificationSenderFactory senderFactory = scope.ServiceProvider.GetRequiredService<INotificationSenderFactory>();

                // verify if the last check was more than 30 seconds ago
                if (DateTime.UtcNow - _lastDbCheck >= _dbCheckInterval)
                {
                    _lastDbCheck = DateTime.UtcNow;

                    //get pending notifications from the database
                    var pendingNotifications = await dbContext.NotificationMessages
                        .Where(n => n.Status == NotificationStatusTypeEnum.Pending ||
                                    (n.Status == NotificationStatusTypeEnum.Failed && n.RetryCount < 3))
                        .OrderBy(n => n.CreatedAt)
                        .Take(10)
                        .ToListAsync(stoppingToken);


                    //if there are no pending notifications, wait 1s and continue
                    if (pendingNotifications.Count == 0)
                    {
                        _workerStatusService.UpdateSuccess();
                        _logger.LogInformation("No pending notifications to process");
                        await Task.Delay(1000, stoppingToken);
                        continue;
                    }

                    // Enqueue pending notifications
                    _logger.LogInformation("Found {Count} pending notifications to process", pendingNotifications.Count);
                    foreach (var notification in pendingNotifications)
                    {
                        notification.Status = NotificationStatusTypeEnum.Processing;

                        _queue.Enqueue(new NotificationRequest
                        {
                            Id = notification.Id,
                            Channel = notification.Channel,
                            Recipient = notification.Recipient,
                            Subject = notification.Subject,
                            Message = notification.Message
                        });
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }

                // Dequeue and process notifications
                if (_queue.TryDequeue(out var request))
                {
                    INotificationSender sender = senderFactory.GetSender(request!.Channel);
                    AsyncRetryPolicy retryPolicy = NotificationRetryPolicies.GetDefaultRetryPolicy(_logger);

                    try
                    {
                        await retryPolicy.ExecuteAsync(async () =>
                        {
                            await sender.SendAsync(request);
                        });
                        NotificationMessage? entity = await dbContext.NotificationMessages.FindAsync(request.Id);
                        if (entity != null)
                        {
                            entity.Status = NotificationStatusTypeEnum.Sent;
                            entity.SentAt = DateTime.UtcNow;
                            await dbContext.SaveChangesAsync();
                        }
                        _logger.LogInformation("Notification sent successfully to {Recipient}", request.Recipient);
                        _workerStatusService.UpdateSuccess();

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
                        _workerStatusService.UpdateFailure();
                    }
                }
                else
                {
                    _logger.LogInformation("No notifications to process");
                }
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
