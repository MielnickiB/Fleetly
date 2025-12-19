using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.LocationDtos;

namespace FleetlyBackend.Services.LocationService
{
    public interface ILocationService
    {
        Task<PagedResult<LocationResponseDto>> GetAll();
        Task<LocationResponseDto?> GetById(int id);
        Task<LocationResponseDto> Create(LocationCreateDto dto);
        Task<LocationResponseDto> Update(int id, LocationUpdateDto dto);
        Task<bool> Deactivate(int id);
    }
}
