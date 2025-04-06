using Microsoft.Extensions.Options;
using NotificationService.Configuration;
using NotificationService.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace NotificationService.Services
{
    public class EmailNotificationSender : INotificationSender
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailNotificationSender(IOptions<SmtpSettings> smtpOptions)
        {
            _smtpSettings = smtpOptions.Value;
        }

        public async Task SendAsync(string recipient, string subject, string message)
        {
            using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Password),
                EnableSsl = true
            };
            
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                Subject = subject,
                Body = message,
                IsBodyHtml = false
            };

            mailMessage.To.Add(new MailAddress(recipient));
            await client.SendMailAsync(mailMessage);

            Console.WriteLine("Email Sent");
        }
    }
}
