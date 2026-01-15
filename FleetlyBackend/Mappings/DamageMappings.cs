using Fleetly.Shared.Dto.DamageDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Mappings
{
    public static class DamageMappings
    {
        public static DamageResponseDto ToResponseDto(this Damage d)
        => new()
        {
            Id = d.Id,
            Side = d.Side,
            Part = d.Part,
            Type = d.Type,
            Description = d.Description ?? string.Empty,
            PhotoUrl = d.PhotoUrl,
            CreatedAt = d.CreatedAt,
            IsFixed = d.IsFixed,
            FixedAt = d.FixedAt,
            FixedByProtocolId = d.FixedByProtocolId
        };
    }
}
