using NotificationService.Models;

namespace NotificationService.Services.Interfaces
{
    public interface INotificationQueue
    {
        void Enqueue(NotificationRequest request);
        bool TryDequeue(out NotificationRequest? request);
        IEnumerable<NotificationRequest> GetPendingNotifications();
        NotificationRequest? GetPendingNotificationById(Guid id);
    }
}
