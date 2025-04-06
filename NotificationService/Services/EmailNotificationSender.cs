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
        private readonly ILogger<EmailNotificationSender> _logger;
        public EmailNotificationSender(IOptions<SmtpSettings> smtpOptions, ILogger<EmailNotificationSender> logger)
        {
            _smtpSettings = smtpOptions.Value;
            _logger = logger;
        }

        public async Task SendAsync(string recipient, string subject, string message)
        {
            try
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

                _logger.LogInformation("Email sent to {Recipient} with subject {Subject}", recipient, subject);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", recipient);
                throw;
            }
        }
    }
}