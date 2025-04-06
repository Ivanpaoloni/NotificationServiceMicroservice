using NotificationService.Services.Interfaces;

namespace NotificationService.Services
{
    public class SmsNotificationSender : INotificationSender
    {
        public Task SendAsync(string recipient, string subject, string message)
        {
            Console.WriteLine($"Sending SMS to {recipient}:{subject}, {message}");
            return Task.CompletedTask;
        }
    }
}
