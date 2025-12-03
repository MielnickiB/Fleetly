using Fleetly.Shared.Dto.CarBrandDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Mappings
{
    public static class CarBrandMappings
    {
        public static CarBrandResponseDto ToResponseDto(this CarBrand brand)
            => new()
            {
                Id = brand.Id,
                BrandName = brand.BrandName
            };
    }
}
