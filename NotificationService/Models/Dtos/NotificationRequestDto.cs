using NotificationService.Domain.Enums;

namespace NotificationService.Models.Dtos
{
    public class NotificationRequestDto
    {
        public NotificationChannelTypeEnum Channel { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
