using Microsoft.AspNetCore.Mvc;
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

        public NotificationsController(IServiceProvider serviceProvider, INotificationService notificationService, ILogger<NotificationsController> logger)
        {
            _serviceProvider = serviceProvider;
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpGet("pending")]
        public IActionResult GetPendingNotifications()
        {
            IEnumerable<NotificationMessageDto> pending = _notificationService.GetPendingNotifications();

            if (pending == null)
            {
                return NotFound(new { success = "false", status = "Not Found" });
            }

            else if (pending.Count() == 0)
            {
                return NoContent();
            }

            return Ok(pending);
        }


        [HttpGet("{id}")]
        public IActionResult GetNotificationById(Guid id)
        {
            NotificationMessageDto? notification = _notificationService.GetStoredNotificationById(id).Result;

            if (notification == null)
            {
                return NotFound(new { success = "false", status = "Not Found" });
            }

            return Ok(notification);
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
    }
}