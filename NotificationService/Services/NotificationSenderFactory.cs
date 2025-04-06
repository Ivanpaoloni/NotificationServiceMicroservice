using NotificationService.Models;
using NotificationService.Services.Interfaces;

namespace NotificationService.Services
{
    public class NotificationSenderFactory : INotificationSenderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationSenderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        //crea la instancia del sender correspondiente al channel
        public INotificationSender GetSender(NotificationChannelTypeEnum channel)
        {
            return channel switch
            {
                NotificationChannelTypeEnum.Email => _serviceProvider.GetRequiredService<EmailNotificationSender>(),
                NotificationChannelTypeEnum.SMS => _serviceProvider.GetRequiredService<SmsNotificationSender>(),
                _ => throw new NotImplementedException($"No sender implemented for {channel}")
            };
        }
    }
}
