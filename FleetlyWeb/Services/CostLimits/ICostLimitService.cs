using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.CostLimitDtos;
using Fleetly.Shared.Client;

namespace FleetlyWeb.Services.CostLimits
{
    public interface ICostLimitService
    {
        Task<ApiResponse<PagedResult<CostLimitResponseDto>?>> GetAllAsync(bool includeInactive);
        Task<ApiResponse<CostLimitResponseDto?>> GetAsync(int id);
        Task<ApiResponse<CostLimitResponseDto?>> CreateAsync(CostLimitCreateDto dto);
        Task<ApiResponse<CostLimitResponseDto?>> UpdateAsync(int id, CostLimitUpdateDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
