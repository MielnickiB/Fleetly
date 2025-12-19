using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.LocationDtos;

namespace FleetlyWeb.Services.Locations
{
    public interface ILocationService
    {
        Task<ApiResponse<PagedResult<LocationResponseDto>>> GetAllLocationsAsync();
        Task<ApiResponse<LocationResponseDto?>> GetLocationByIdAsync(int LocationId);
        Task<ApiResponse<LocationResponseDto>> CreateLocationAsync(LocationCreateDto dto);
        Task<ApiResponse<LocationResponseDto>> UpdateLocationAsync(int id, LocationUpdateDto dto);
        Task<ApiResponse<bool>> DeactivateLocationAsync(int id);
    }
}
