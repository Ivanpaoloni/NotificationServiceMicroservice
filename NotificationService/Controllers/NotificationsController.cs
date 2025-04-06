using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Services.Interfaces;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INotificationService _notificationService;

        public NotificationsController(IServiceProvider serviceProvider, INotificationService notificationService)
        {
            _serviceProvider = serviceProvider;
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            if (request == null) return BadRequest("Invalid request");

            await _notificationService.SendAsync(request);

            return Ok(new { success = "true", status = "Sent", channel = request.Channel.ToString(), subject = request.Subject.ToString(), message = request.Message.ToString() });
        }
    }
}
