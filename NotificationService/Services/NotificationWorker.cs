using NotificationService.Policies;
using NotificationService.Services.Interfaces;

namespace NotificationService.Services
{
    public class NotificationWorker : BackgroundService
    {
        private readonly INotificationQueue _queue;
        private readonly ILogger<NotificationWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

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
                if (_queue.TryDequeue(out var request))
                {
                    using var scope = _scopeFactory.CreateScope();
                    var senderFactory = scope.ServiceProvider.GetRequiredService<INotificationSenderFactory>();
                    var sender = senderFactory.GetSender(request!.Channel);

                    var retryPolicy = NotificationRetryPolicies.GetDefaultRetryPolicy(_logger);

                    try
                    {
                        await retryPolicy.ExecuteAsync(async () =>
                        {
                            await sender.SendAsync(request.Recipient, request.Subject, request.Message);
                        });

                        _logger.LogInformation("Notification sent successfully to {Recipient}", request.Recipient);

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Final failure after retries sending notification to {Recipient}", request.Recipient);
                    }
                }
                await Task.Delay(100, stoppingToken); // Pequeño delay para no saturar el CPU
            }
        }
    }
}
