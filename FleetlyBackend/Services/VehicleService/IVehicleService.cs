using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.VehicleDtos;

namespace FleetlyBackend.Services.VehicleService
{
    public interface IVehicleService
    {
        Task<PagedResult<VehicleResponseDto>> GetAll(bool includeInactive);
        Task<VehicleResponseDto?> GetById(int id);
        Task<VehicleResponseDto> Create(VehicleCreateDto dto);
        Task<VehicleResponseDto> Update(int id, VehicleUpdateDto dto);
        Task<bool> Deactivate(int id);
    }
}
