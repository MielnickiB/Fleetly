using FleetlyBackend.Mappings;
using Fleetly.Shared.Dto.ProtocolDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Mappings
{
    public static class ProtocolMappings
    {
        public static ProtocolResponseDto ToResponseDto(this Protocol p)
            => new()
            {
                Id = p.Id,
                OrderId = p.OrderId,
                Type = p.Type,
                CreatedAt = p.CreatedAt,
                Mileage = p.Mileage,
                FuelLevel = p.FuelLevel,
                SignatureUrl = p.SignatureUrl,
                Photos = p.Photos.Select(pp => pp.ToResponseDto()).ToList(),
                Damages = p.Damages.Select(d => d.ToResponseDto()).ToList()
            };

        public static ProtocolPhotoResponseDto ToResponseDto(this ProtocolPhoto pp)
            => new()
            {
                Id = pp.Id,
                ProtocolId = pp.ProtocolId,
                Side = pp.Side,
                PhotoUrl = pp.PhotoUrl
            };

    }
}
