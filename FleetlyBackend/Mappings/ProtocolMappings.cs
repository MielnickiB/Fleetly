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
                CurrentStep = p.CurrentStep,
                Mileage = p.Mileage,
                FuelLevel = p.FuelLevel,
                SignatureUrl = p.SignatureUrl,
                HasRegistrationDocument = p.HasRegistrationDocument,
                HasInsurancePolicy = p.HasInsurancePolicy,
                HasServiceBook = p.HasServiceBook,
                NumberOfKeys = p.NumberOfKeys,
                Notes = p.Notes,
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

        public static ProtocolLiteDto ToLiteDto(this Protocol p)
            => new()
            {
                Id = p.Id,
                Type = p.Type
            };
    }
}
