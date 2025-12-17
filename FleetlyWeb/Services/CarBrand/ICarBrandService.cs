using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.CarBrandDtos;

namespace FleetlyWeb.Services.CarBrand
{
    public interface ICarBrandService
    {
        Task<ApiResponse<PagedResult<CarBrandResponseDto>>> GetAllCarBrandsAsync();
    }
}
