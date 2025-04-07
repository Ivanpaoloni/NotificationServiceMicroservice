using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NotificationService.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;

namespace NotificationService.Healthchecks
{
    public class CustomSmtpHealthCheck : IHealthCheck
    {
        private readonly SmtpSettings _smtpSettings;

        public CustomSmtpHealthCheck(IOptions<SmtpSettings> options)
        {
            _smtpSettings = options.Value;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Password)
                };

                using var tcpClient = new TcpClient();
                tcpClient.Connect(_smtpSettings.Host, _smtpSettings.Port);

                return Task.FromResult(HealthCheckResult.Healthy("SMTP server is reachable"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("SMTP check failed", ex));
            }
        }
    }
}