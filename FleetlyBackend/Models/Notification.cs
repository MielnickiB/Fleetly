using System.ComponentModel.DataAnnotations;
using Fleetly.Shared.Enums;

namespace FleetlyBackend.Models
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public NotificationType Type { get; set; }

        [StringLength(150)]
        public string Title { get; set; } = null!;

        [StringLength(500)]
        public string Message { get; set; } = null!;

        public int? RelatedEntityId { get; set; }
        public RelatedEntityType? RelatedEntity { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime? ReadAt { get; set; }
    }
}
