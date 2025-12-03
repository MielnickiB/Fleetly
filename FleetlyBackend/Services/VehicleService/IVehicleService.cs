using Fleetly.Shared.Dto.VehicleDtos;

namespace FleetlyBackend.Services.VehicleService
{
    public interface IVehicleService
    {
        Task<List<VehicleResponseDto>> GetAll(int page, int pageSize);
        Task<List<VehicleResponseDto>> GetUserVehicles(int userId);
        Task<VehicleResponseDto?> GetById(int id);
        Task<VehicleResponseDto> Create(int userId, VehicleCreateDto dto);
        Task<VehicleResponseDto> Update(int id, VehicleUpdateDto dto, int? userId = null);
        Task<bool> Deactivate(int id, int? userId = null);
    }
}
