using NotificationService.Models;

namespace NotificationService.Services.Interfaces
{
    public interface INotificationSender
    {
        Task SendAsync(NotificationRequest notificationRequest);
    }
}
