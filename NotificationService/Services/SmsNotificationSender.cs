using NotificationService.Services.Interfaces;

namespace NotificationService.Services
{
    public class SmsNotificationSender : INotificationSender
    {
        private readonly ILogger<SmsNotificationSender> _logger;

        public SmsNotificationSender(ILogger<SmsNotificationSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string recipient, string subject, string message)
        {
            _logger.LogInformation("Sending SMS to {Recipient} with subject {Subject} and message {Message}", recipient, subject, message);
            return Task.CompletedTask;
        }
    }
}
