using AutoMapper;
using FluentValidation;
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
        private readonly IMapper _mapper;
        public NotificationService(ILogger<NotificationService> logger, INotificationQueue emailQueue, NotificationDbContext notificationDbContext, IMapper mapper)
        {
            _logger = logger;
            _notificationQueue = emailQueue;
            this._notificationDbContext = notificationDbContext;
            _mapper = mapper;
        }
        public async Task<Guid> SendNotificationAsync(NotificationRequestDto dto)
        {
            new NotificationRequestDtoValidator().ValidateAndThrow(dto);

            NotificationRequest notificationRequest = _mapper.Map<NotificationRequest>(dto);
            notificationRequest.Id = Guid.NewGuid();

            NotificationMessage notification = new NotificationMessage
            {
                Id = notificationRequest.Id,
                Channel = notificationRequest.Channel,
                Recipient = notificationRequest.Recipient,
                Subject = notificationRequest.Subject,
                Message = notificationRequest.Message,
                Status = NotificationStatusTypeEnum.Pending,
                RetryCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationDbContext.NotificationMessages.AddAsync(notification);
            await _notificationDbContext.SaveChangesAsync();

            _notificationQueue.Enqueue(notificationRequest);

            return notificationRequest.Id;
        }
    }
}
