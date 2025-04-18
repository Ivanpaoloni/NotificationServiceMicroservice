using NotificationService.Models;
using NotificationService.Models.Dtos;

namespace NotificationService.Services.Interfaces
{
    public interface INotificationService
    {
        IEnumerable<NotificationMessageDto> GetPendingNotifications();
        Task<NotificationMessageDto?> GetStoredNotificationById(Guid id);
        Task<Guid> SendNotificationAsync(NotificationRequestDto dto);
    }
}
