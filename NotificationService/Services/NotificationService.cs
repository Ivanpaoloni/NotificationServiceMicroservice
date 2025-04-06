using NotificationService.Models;
using NotificationService.Services.Interfaces;

namespace NotificationService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationSenderFactory _notificationSenderFactory;
        private readonly ILogger<NotificationService> _logger;
        public NotificationService(INotificationSenderFactory notificationSenderFactory, ILogger<NotificationService> logger)
        {
            _notificationSenderFactory = notificationSenderFactory;
            _logger = logger;
        }

        public async Task SendAsync(NotificationRequest request)
        {
            var sender = _notificationSenderFactory.GetSender(request.Channel);
            _logger.LogInformation("Sending notification using {Channel} channel", request.Channel.ToString());
            await sender.SendAsync(request.Recipient, request.Subject, request.Message);
        }
    }
}
