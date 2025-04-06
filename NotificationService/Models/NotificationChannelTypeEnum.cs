using System.ComponentModel;

namespace NotificationService.Models
{
    public enum NotificationChannelTypeEnum
    {
        [Description("Email")]
        Email = 1,
        [Description("SMS")]
        SMS = 2
    }
}
