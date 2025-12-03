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
                VehicleId = d.VehicleId,
                ProtocolId = d.ProtocolId,
                DamageSide = d.DamageSide,
                DamageLocation = d.DamageLocation,
                DamagePart = d.DamagePart,
                DamageType = d.DamageType,
                Description = d.Description,
                PhotoUrl = d.PhotoUrl,
                CreatedAt = d.CreatedAt
            };
    }
}
