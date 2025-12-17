using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.BrandModelDtos;

namespace FleetlyWeb.Services.BrandModel
{
    public interface IBrandModelService
    {
        Task<ApiResponse<PagedResult<BrandModelResponseDto>>> GetAllBrandModelsAsync();
    }
}
