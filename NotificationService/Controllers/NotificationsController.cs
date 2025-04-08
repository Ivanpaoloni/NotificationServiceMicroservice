using Humanizer;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Models.Dtos;
using NotificationService.Services.Interfaces;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationsController> _logger;
        private readonly INotificationService _notificationService;
        private readonly INotificationQueue _notificationQueue;

        public NotificationsController(IServiceProvider serviceProvider, INotificationService notificationService, INotificationQueue notificationQueue, ILogger<NotificationsController> logger)
        {
            _serviceProvider = serviceProvider;
            _notificationService = notificationService;
            _notificationQueue = notificationQueue;
            _logger = logger;
        }

        [HttpGet("pending")]
        public IActionResult GetPendingNotifications()
        {
            var pending =_notificationQueue.GetPendingNotifications();
            return Ok(new { success = "true", status = "Pending", pending });
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Llama al servicio que crea el registro en DB y encola la notificación.
                var notificationId = await _notificationService.SendNotificationAsync(dto);

                return Ok(new { NotificationId = notificationId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification");
                return StatusCode(400, ex.Message);
            }
        }

        // Podrías agregar otros endpoints, como un GET para ver notificaciones pendientes,
        // o para consultar el estado de una notificación en particular.
    }
}