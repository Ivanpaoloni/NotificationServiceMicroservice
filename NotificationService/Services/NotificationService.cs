using NotificationService.Models;
using NotificationService.Services.Interfaces;

namespace NotificationService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationSenderFactory _notificationSenderFactory;

        public NotificationService(INotificationSenderFactory notificationSenderFactory)
        {
            _notificationSenderFactory = notificationSenderFactory;
        }

        public async Task SendAsync(NotificationRequest request)
        {
            var sender = _notificationSenderFactory.GetSender(request.Channel);
            await sender.SendAsync(request.Recipient, request.Subject, request.Message);
        }
    }
}
