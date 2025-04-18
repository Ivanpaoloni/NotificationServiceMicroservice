using NotificationService.Models;
using NotificationService.Services.Interfaces;
using System.Collections.Concurrent;

namespace NotificationService.Services
{
    public class InMemoryNotificationQueue : INotificationQueue
    {
        private readonly ConcurrentQueue<NotificationRequest> _queue = new();
        public void Enqueue(NotificationRequest notification) => _queue.Enqueue(notification);
        public bool TryDequeue(out NotificationRequest notification) => _queue.TryDequeue(out notification!);
        public IEnumerable<NotificationRequest> GetPendingNotifications() => _queue.ToList();
        public NotificationRequest? GetPendingNotificationById(Guid id)
        {
            return _queue.FirstOrDefault(n => n.Id == id);
        }
    }

}
