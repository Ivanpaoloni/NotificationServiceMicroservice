using Polly;
using Polly.Retry;

namespace NotificationService.Policies
{
    public static class NotificationRetryPolicies
    {
        public static AsyncRetryPolicy GetDefaultRetryPolicy(ILogger logger)
        {
            return Policy
                .Handle<Exception>().WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        logger.LogWarning(exception, "Retry {RetryCount} after {Delay} due to error: {Message}", retryCount, timeSpan, exception.Message);
                    });
        }
    }
}
