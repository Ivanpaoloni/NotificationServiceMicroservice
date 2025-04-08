using Microsoft.Extensions.Diagnostics.HealthChecks;
using NotificationService.Infrastructure;

namespace NotificationService.Healthchecks
{
    public class CustomDbHealthCheck : IHealthCheck
    {
        private readonly NotificationDbContext _dbContext;

        public CustomDbHealthCheck(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
                return canConnect 
                    ? HealthCheckResult.Healthy("Database is reachable") 
                    : HealthCheckResult.Unhealthy("Database is not reachable");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database check failed", ex);
            }
        }
    }
}
