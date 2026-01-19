using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.LocationDtos;
using Fleetly.Shared.Client;

namespace FleetlyWeb.Services.Locations
{
    public interface ILocationService
    {
        Task<ApiResponse<PagedResult<LocationResponseDto>>> GetAllLocationsAsync(bool includeInactive);
        Task<ApiResponse<LocationResponseDto?>> GetLocationByIdAsync(int locationId);
        Task<ApiResponse<LocationResponseDto>> CreateLocationAsync(LocationCreateDto dto);
        Task<ApiResponse<LocationResponseDto>> UpdateLocationAsync(int id, LocationUpdateDto dto);
        Task<ApiResponse<bool>> DeactivateLocationAsync(int id);
    }
}
