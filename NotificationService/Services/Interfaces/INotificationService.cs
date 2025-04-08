using NotificationService.Models;
using NotificationService.Models.Dtos;

namespace NotificationService.Services.Interfaces
{
    public interface INotificationService
    {
        Task<Guid> SendNotificationAsync(NotificationRequestDto dto);
    }
}
