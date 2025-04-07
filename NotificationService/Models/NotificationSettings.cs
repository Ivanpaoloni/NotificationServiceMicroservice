namespace NotificationService.Models
{
    public class NotificationSettings
    {
        public int MaxRetries { get; set; }
        public int RetryDelayInSeconds { get; set; }
    }
}
