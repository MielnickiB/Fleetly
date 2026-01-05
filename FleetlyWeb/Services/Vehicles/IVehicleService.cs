using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.VehicleDtos;

namespace FleetlyWeb.Services.Vehicles
{
    public interface IVehicleService
    {
        Task<ApiResponse<PagedResult<VehicleResponseDto>>> GetAllVehiclesAsync();
        Task<ApiResponse<VehicleResponseDto?>> GetVehicleByIdAsync(int vehicleId);
        Task<ApiResponse<VehicleResponseDto>> CreateVehicleAsync(VehicleCreateDto dto);
        Task<ApiResponse<VehicleResponseDto>> UpdateVehicleAsync(int id, VehicleUpdateDto dto);
        Task<ApiResponse<bool>> DeactivateVehicleAsync(int id);
    }
}
