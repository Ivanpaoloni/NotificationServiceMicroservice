using NotificationService.Services.Interfaces;

namespace NotificationService.Services
{
    public class EmailWorker : BackgroundService
    {
        private readonly INotificationQueue _emailQueue;
        private readonly ILogger<EmailWorker> _logger;
        private readonly EmailNotificationSender _sender;

        public EmailWorker(INotificationQueue queue, EmailNotificationSender sender, ILogger<EmailWorker> logger)
        {
            _emailQueue = queue;
            _sender = sender;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_emailQueue.TryDequeue(out var request))
                {
                    try
                    {
                        await _sender.SendAsync(request!.Recipient, request.Subject, request.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send email to {Recipient}", request!.Recipient);
                        // acá podrías reintentar o guardar en algún lado
                    }
                }
                else
                {
                    await Task.Delay(500, stoppingToken); // evitar busy wait
                }
            }
        }
    }

}
