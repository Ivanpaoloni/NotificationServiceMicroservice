using NotificationService.Domain.Enums;
using System.Text.Json.Serialization;

namespace NotificationService.Models.Dtos
{
    public class NotificationMessageDto
    {
        public Guid Id { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NotificationChannelTypeEnum Channel { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NotificationStatusTypeEnum Status { get; set; }
        public int RetryCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastTriedAt { get; set; }
        public DateTime? SentAt { get; set; }
    }
}
