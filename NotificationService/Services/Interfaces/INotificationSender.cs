﻿namespace NotificationService.Services.Interfaces
{
    public interface INotificationSender
    {
        Task SendAsync(string recipient, string subject, string message);
    }
}
