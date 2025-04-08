using NotificationService.Domain.Enums;

namespace NotificationService.Services.Interfaces
{
    public interface INotificationSenderFactory
    {
        INotificationSender GetSender(NotificationChannelTypeEnum channel);
    }
}
