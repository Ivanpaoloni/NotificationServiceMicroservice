using System.ComponentModel;

namespace NotificationService.Domain.Enums
{
    public enum NotificationChannelTypeEnum
    {
        [Description("Email")]
        Email = 1,
        [Description("SMS")]
        SMS = 2
    }
}
