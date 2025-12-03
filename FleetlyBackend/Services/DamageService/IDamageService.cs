using Fleetly.Shared.Dto.DamageDtos;

namespace FleetlyBackend.Services.DamageService
{
    public interface IDamageService
    {
        Task<List<DamageResponseDto>> GetForVehicle(int vehicleId);
        Task<DamageResponseDto?> Get(int id);
        Task<DamageResponseDto> Create(DamageCreateDto dto);
        Task<DamageResponseDto> Update(int id, DamageUpdateDto dto);
        Task<bool> Delete(int id);
    }
}
