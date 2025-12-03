using Fleetly.Shared.Dto.BrandModelDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Mappings
{
    public static class BrandModelMappings
    {
        public static BrandModelResponseDto ToResponseDto(this BrandModel bm)
            => new()
            {
                Id = bm.Id,
                ModelName = bm.ModelName,
                CarBrandId = bm.CarBrandId,
                CarBrandName = bm.CarBrand.BrandName
            };
    }
}
