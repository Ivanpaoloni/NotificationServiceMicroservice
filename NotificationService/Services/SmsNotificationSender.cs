using Microsoft.Extensions.Options;
using NotificationService.Models;
using NotificationService.Services.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace NotificationService.Services
{
    public class SmsNotificationSender : INotificationSender
    {
        private readonly ILogger<SmsNotificationSender> _logger;
        private readonly TwilioSettings _settings;

        public SmsNotificationSender(ILogger<SmsNotificationSender> logger, IOptions<TwilioSettings> options)
        {
            _logger = logger;
            _settings = options.Value;

            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);
        }

        public async Task SendAsync(NotificationRequest request)
        {
            try
            {
                var message = await MessageResource.CreateAsync(
                    to: new PhoneNumber(request.Recipient),
                    from: new PhoneNumber(_settings.FromPhoneNumber),
                    body: request.Message);

                _logger.LogInformation("SMS enviado a {Recipient}. SID: {Sid}", request.Recipient, message.Sid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar SMS a {Recipient}", request.Recipient);
                throw;
            }
        }
    }
}
