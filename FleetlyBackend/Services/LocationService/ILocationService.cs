using Fleetly.Shared.Dto.LocationDtos;

namespace FleetlyBackend.Services.LocationService
{
    public interface ILocationService
    {
        Task<List<LocationResponseDto>> GetAll(int page = 1, int pageSize = 10);
        Task<LocationResponseDto?> GetById(int id);
        Task<LocationResponseDto> Create(LocationCreateDto dto);
        Task<LocationResponseDto> Update(int id, LocationUpdateDto dto);
        Task<bool> Deactivate(int id);
    }
}
