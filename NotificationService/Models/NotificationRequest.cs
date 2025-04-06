namespace NotificationService.Models
{
    public class NotificationRequest
    {
        public NotificationChannelTypeEnum Channel { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
