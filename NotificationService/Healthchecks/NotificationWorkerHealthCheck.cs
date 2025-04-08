using Microsoft.Extensions.Diagnostics.HealthChecks;
using NotificationService.Services;

namespace NotificationService.Healthchecks
{
    public class NotificationWorkerHealthCheck : IHealthCheck
    {
        private readonly WorkerStatusService _status;

        public NotificationWorkerHealthCheck(WorkerStatusService status)
        {
            _status = status;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var timeSinceLastRun = DateTime.UtcNow - _status.LastExecutionTime;
            var healthy = timeSinceLastRun < TimeSpan.FromMinutes(5) && !_status.LastExecutionFailed;

            return Task.FromResult(healthy
                ? HealthCheckResult.Healthy("Worker is active.")
                : HealthCheckResult.Unhealthy("Worker might be inactive or failed."));
        }
    }
}
