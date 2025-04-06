using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Services;
using NotificationService.Services.Interfaces;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        
        public NotificationsController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            INotificationSender? sender = request.Channel switch
            {
                NotificationChannelTypeEnum.Email => _serviceProvider.GetService<EmailNotificationSender>(),
                NotificationChannelTypeEnum.SMS => _serviceProvider.GetService<SmsNotificationSender>(),
                _ => throw new NotImplementedException($"Notification channel {request.Channel} is not implemented")
            };

            await sender!.SendAsync(request.Recipient, request.Subject, request.Message);
            return Ok(new { status = "Sent", channel = request.Channel.ToString(), message = request.Message.ToString() });
        }
    }
}
