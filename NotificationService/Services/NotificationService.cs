using FluentValidation;
using NotificationService.Models;
using NotificationService.Services.Interfaces;
using NotificationService.Validations;

namespace NotificationService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly INotificationQueue _notificationQueue;
        public NotificationService(ILogger<NotificationService> logger, INotificationQueue emailQueue)
        {
            _logger = logger;
            _notificationQueue = emailQueue;
        }

        public Task SendAsync(NotificationRequest request)
        {
            new NotificationRequestValidator().ValidateAndThrow(request);

            _notificationQueue.Enqueue(request);
            _logger.LogInformation("Sending notification using {Channel} channel", request.Channel.ToString());

            return Task.CompletedTask;
        }
    }
}
