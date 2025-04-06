using NotificationService.Models;

namespace NotificationService.Services.Interfaces
{
    public interface INotificationSenderFactory
    {
        INotificationSender GetSender(NotificationChannelTypeEnum channel);
    }
}
