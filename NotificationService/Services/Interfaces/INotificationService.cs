using NotificationService.Models;
using NotificationService.Models.Dtos;

namespace NotificationService.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendAsync(NotificationRequest request);
        Task<Guid> SendNotificationAsync(NotificationRequestDto dto);
    }
}
