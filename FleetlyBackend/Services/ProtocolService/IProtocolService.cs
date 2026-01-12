using Fleetly.Shared.Dto.ProtocolDtos;

namespace FleetlyBackend.Services.ProtocolService
{
    public interface IProtocolService
    {
        public Task<int> StartProtocolAsync(ProtocolInitDto dto);
        public Task AddProtocolPhotoAsync(ProtocolPhotoDto dto);
        public Task AddDamageAsync(DamageCreateDto dto);
        public Task FinishProtocolAsync(ProtocolFinishDto dto);
    }
}
