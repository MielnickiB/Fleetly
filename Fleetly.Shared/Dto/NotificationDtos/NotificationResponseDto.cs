using Fleetly.Shared.Enums;

namespace Fleetly.Shared.Dto.NotificationDtos
{
    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }

        public int? RelatedEntityId { get; set; }
        public RelatedEntityType? RelatedEntity { get; set; }
    }
}
