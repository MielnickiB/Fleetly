using FleetlyBackend.Models;
using Fleetly.Shared.Dto.NotificationDtos;

namespace FleetlyBackend.Mappings
{
    public static class NotificationMappings
    {
        public static NotificationResponseDto ToResponseDto(this Notification n)
            => new()
            {
                Id = n.Id,
                Type = n.Type,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt,
                RelatedEntityId = n.RelatedEntityId,
                RelatedEntity = n.RelatedEntity
            };
    }
}
