using Fleetly.Shared.Dto.VehicleDtos;
using Fleetly.Shared.Dto.UserDetailsDtos;
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
                UserDetails = new UserDetailsDto
                {
                    Name = v.User.Details.Name,
                    Surname = v.User.Details.Surname,
                    PhoneNumber = v.User.Details.PhoneNumber,
                    Company = v.User.Details?.Company
                },
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
                VIN = v.VIN,
                Year = v.Year,
                Details = v.Details,
                IsActive = v.IsActive
            };
    }
}
