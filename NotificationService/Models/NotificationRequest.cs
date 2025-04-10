﻿using NotificationService.Domain.Enums;

namespace NotificationService.Models
{
    public class NotificationRequest
    {
        public Guid Id { get; set; }
        public NotificationChannelTypeEnum Channel { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
