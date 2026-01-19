using Fleetly.Shared.Client;
using Fleetly.Shared.Dto.AvailabilityDtos;

namespace FleetlyMobile.Services.Availability
{
    public interface IAvailabilityService
    {
        Task<ApiResponse<List<AvailabilityResponseDto>?>> GetByRangeAsync(DateOnly start, DateOnly end);
        Task<ApiResponse<AvailabilityResponseDto?>> GetAsync(int id);
        Task<ApiResponse<List<AvailabilityResponseDto>?>> CreateAsync(AvailabilityCreateDto dto);
        Task<ApiResponse<AvailabilityResponseDto?>> UpdateAsync(int id, AvailabilityUpdateDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<bool>> DeleteRangeAsync(DateOnly start, DateOnly end);
    }
}
