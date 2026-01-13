using Fleetly.Shared.Dto.DamageDtos;
using Fleetly.Shared.Dto.ProtocolDtos;

namespace FleetlyBackend.Services.ProtocolService
{
    public interface IProtocolService
    {
        public Task<ProtocolResponseDto> GetProtocolByOrderIdAsync(int orderId);
        public Task<ProtocolResponseDto> StartProtocolAsync(ProtocolInitDto dto);
        public Task<ProtocolResponseDto> AddProtocolPhotoAsync(ProtocolPhotoDto dto);
        public Task<ProtocolResponseDto> AddDamageAsync(DamageCreateDto dto);
        public Task<ProtocolResponseDto> DeleteDamageAsync(int damageId);
        public Task<ProtocolResponseDto> MarkDamageAsFixedAsync(int damageId, int currentProtocolId);
        public Task<ProtocolResponseDto> FinishProtocolAsync(ProtocolFinishDto dto);
    }
}
