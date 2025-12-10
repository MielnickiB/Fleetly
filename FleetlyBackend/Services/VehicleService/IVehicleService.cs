using Fleetly.Shared.Dto.VehicleDtos;

namespace FleetlyBackend.Services.VehicleService
{
    public interface IVehicleService
    {
        Task<List<VehicleResponseDto>> GetAll(int page = 1, int pageSize = 10);
        Task<VehicleResponseDto?> GetById(int id);
        Task<VehicleResponseDto> Create(VehicleCreateDto dto);
        Task<VehicleResponseDto> Update(int id, VehicleUpdateDto dto);
        Task<bool> Deactivate(int id);
    }
}
