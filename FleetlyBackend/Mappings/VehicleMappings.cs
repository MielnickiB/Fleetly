using Fleetly.Shared.Dto.VehicleDtos;
using Fleetly.Shared.Dto.BrandModelDtos;
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
                UserFullName = v.User.Details.Name + " " + v.User.Details.Surname,
                BrandModelId = v.BrandModelId,
                BrandModel = new BrandModelResponseDto
                {
                    Id = v.BrandModel.Id,
                    CarBrandName = v.BrandModel.CarBrand.BrandName,
                    CarBrandId = v.BrandModel.CarBrandId,
                    ModelName = v.BrandModel.ModelName,
                },
                RegistrationNumber = v.RegistrationNumber,
                Mileage = v.Mileage,
                FuelType = v.FuelType,
                VIN = v.VIN,
                Year = v.Year,
                Details = v.Details,
                IsActive = v.IsActive
            };
    }
}
