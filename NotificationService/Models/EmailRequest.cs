namespace NotificationService.Models
{
    public class EmailRequest
    {
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int RetryCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public EmailRequest(string recipient, string subject, string body)
        {
            Recipient = recipient;
            Subject = subject;
            Body = body;
            RetryCount = 0;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
