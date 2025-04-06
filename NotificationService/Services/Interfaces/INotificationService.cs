using NotificationService.Models;

namespace NotificationService.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendAsync(NotificationRequest request);
    }
}
