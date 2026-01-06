using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.BrandModelDtos;
using Fleetly.Shared.Client;

namespace FleetlyWeb.Services.BrandModel
{
    public interface IBrandModelService
    {
        Task<ApiResponse<PagedResult<BrandModelResponseDto>>> GetAllBrandModelsAsync();
    }
}
