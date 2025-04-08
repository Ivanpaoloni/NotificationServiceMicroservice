using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Entities
{
    public class NotificationMessage
    {
        public Guid Id { get; set; }
        public NotificationChannelTypeEnum Channel { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public NotificationStatusTypeEnum Status { get; set; }
        public int RetryCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
    }
}
