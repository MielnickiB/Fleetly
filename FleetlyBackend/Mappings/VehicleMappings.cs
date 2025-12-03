using Fleetly.Shared.Dto.VehicleDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Mappings
{
    public static class VehicleMappings
    {
        public static VehicleResponseDto ToResponseDto(this Vehicle v)
            => new()
            {
                Id = v.Id,
                UserId = v.UserId,
                BrandModelId = v.BrandModelId,
                RegistrationNumber = v.RegistrationNumber,
                Mileage = v.Mileage,
                VIN = v.VIN,
                Year = v.Year,
                Details = v.Details,
                IsActive = v.IsActive
            };
    }
}
