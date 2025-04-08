using Microsoft.Extensions.Options;
using NotificationService.Configuration;
using NotificationService.Models;
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

        public async Task SendAsync(NotificationRequest request)
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
                    Subject = request.Subject,
                    Body = request.Message,
                    IsBodyHtml = false
                };

                mailMessage.To.Add(new MailAddress(request.Recipient));
                await client.SendMailAsync(mailMessage);

                _logger.LogInformation("Email sent to {Recipient} with subject {Subject}", request.Recipient, request.Subject);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", request.Recipient);
                throw;
            }
        }
    }
}