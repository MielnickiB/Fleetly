using Fleetly.Shared.Dto.LocationDtos;

namespace FleetlyBackend.Services.LocationService
{
    public interface ILocationService
    {
        Task<List<LocationResponseDto>> GetAll(int page, int pageSize);
        Task<List<LocationResponseDto>> GetUserLocations(int userId);
        Task<LocationResponseDto?> GetById(int id);
        Task<LocationResponseDto> Create(LocationCreateDto dto, int currentUserId);
        Task<LocationResponseDto> Update(int id, LocationUpdateDto dto, int? currentUserId = null);
        Task<bool> Deactivate(int id, int? currentUserId = null);
    }
}
