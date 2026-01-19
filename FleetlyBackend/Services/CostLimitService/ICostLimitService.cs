using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.CostLimitDtos;

namespace FleetlyBackend.Services.CostLimitService
{
    public interface ICostLimitService
    {
        Task<PagedResult<CostLimitResponseDto>> GetAll(bool includeInactive);
        Task<CostLimitResponseDto?> Get(int id);
        Task<CostLimitResponseDto> GetByRangeOfKm(int rangeOfKm);
        Task<CostLimitResponseDto> Create(CostLimitCreateDto dto);
        Task<CostLimitResponseDto> Update(int id, CostLimitUpdateDto dto);
        Task<bool> Delete(int id);
    }
}
