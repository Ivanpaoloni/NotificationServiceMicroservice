namespace NotificationService.Services
{
    public class WorkerStatusService
    {
        // This class is used for healthcheck to track the status of the worker. 
        public DateTime LastExecutionTime { get; set; } = DateTime.MinValue;
        public bool LastExecutionFailed { get; set; } = false;

        public void UpdateSuccess()
        {
            LastExecutionTime = DateTime.UtcNow;
            LastExecutionFailed = false;
        }

        public void UpdateFailure()
        {
            LastExecutionTime = DateTime.UtcNow;
            LastExecutionFailed = true;
        }
    }
}
