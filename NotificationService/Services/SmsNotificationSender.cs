using NotificationService.Models;
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

        public Task SendAsync(NotificationRequest request)
        {
            _logger.LogInformation("Sending SMS to {Recipient} with subject {Subject} and message {Message}", request.Recipient, request.Subject, request.Message);
            return Task.CompletedTask;
        }
    }
}
