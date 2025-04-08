using Microsoft.Extensions.Diagnostics.HealthChecks;
using Twilio;
using Twilio.Rest.Api.V2010;

public class TwilioHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;

    public TwilioHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var accountSid = _configuration["Twilio:AccountSid"];
        var authToken = _configuration["Twilio:AuthToken"];

        try
        {
            TwilioClient.Init(accountSid, authToken);

            var account = AccountResource.Fetch(pathSid: accountSid);

            if (account.Status == AccountResource.StatusEnum.Active)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Twilio account is active."));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Unhealthy($"Twilio account is not active: {account.Status}"));
            }
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"Twilio health check failed: {ex.Message}"));
        }
    }
}
