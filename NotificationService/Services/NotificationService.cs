using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationService.Infrastructure;
using NotificationService.Models;
using NotificationService.Models.Dtos;
using NotificationService.Services.Interfaces;
using NotificationService.Validations;

namespace NotificationService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly NotificationDbContext _notificationDbContext;
        private readonly INotificationQueue _notificationQueue;
        public NotificationService(ILogger<NotificationService> logger, INotificationQueue emailQueue, NotificationDbContext notificationDbContext)
        {
            _logger = logger;
            _notificationQueue = emailQueue;
            this._notificationDbContext = notificationDbContext;
        }
        public async Task<Guid> SendNotificationAsync(NotificationRequestDto dto)
        {
            //new NotificationRequestValidator().ValidateAndThrow(dto); // Si querés validar el DTO

            var id = Guid.NewGuid();

            var notification = new NotificationMessage
            {
                Id = id,
                Channel = dto.Channel,
                Recipient = dto.Recipient,
                Subject = dto.Subject,
                Message = dto.Message,
                Status = NotificationStatusTypeEnum.Pending,
                RetryCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationDbContext.NotificationMessages.AddAsync(notification);
            await _notificationDbContext.SaveChangesAsync();

            _notificationQueue.Enqueue(new NotificationRequest
            {
                Id = id,
                Channel = dto.Channel,
                Recipient = dto.Recipient,
                Subject = dto.Subject,
                Message = dto.Message
            });

            return id;
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
