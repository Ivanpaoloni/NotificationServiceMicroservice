using Polly;
using Polly.Retry;

namespace NotificationService.Policies
{
    public static class NotificationRetryPolicies
    {
        public static AsyncRetryPolicy GetDefaultRetryPolicy(ILogger logger)
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        logger.LogWarning(exception,
                            "Retry {RetryCount} after {Delay} due to: {Message}",
                            retryCount, timespan, exception.Message);
                    });
        }
    }
}
