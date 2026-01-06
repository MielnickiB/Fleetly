using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.CarBrandDtos;
using Fleetly.Shared.Client;

namespace FleetlyWeb.Services.CarBrand
{
    public interface ICarBrandService
    {
        Task<ApiResponse<PagedResult<CarBrandResponseDto>>> GetAllCarBrandsAsync();
    }
}
