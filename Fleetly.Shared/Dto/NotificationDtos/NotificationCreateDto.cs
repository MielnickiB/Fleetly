using Fleetly.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.NotificationDtos
{
    public class NotificationCreateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Id użytkownika, musi być dodatnią wartością.")]
        public int UserId { get; set; }
        [Required]
        public NotificationType Type { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Tytuł nie może przekraczać 150 znaków.")]
        public string Title { get; set; } = null!;
        [Required]
        [StringLength(500, ErrorMessage = "Wiadomość nie może przekraczać 500 znaków.")]
        public string Message { get; set; } = null!;

        public int? RelatedEntityId { get; set; }
        public RelatedEntityType? RelatedEntity { get; set; }
    }
}
