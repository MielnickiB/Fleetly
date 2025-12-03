using Fleetly.Shared.Dto.AvailabilityDtos;

namespace FleetlyBackend.Services.AvailabilityService
{
    public interface IAvailabilityService
    {
        Task<List<AvailabilityResponseDto>> GetAll(int page, int pageSize);
        Task<List<AvailabilityResponseDto>> GetWorkerAvailability(int workerId);
        Task<AvailabilityResponseDto?> Get(int id);
        Task<AvailabilityResponseDto> Create(int workerId, AvailabilityCreateDto dto);
        Task<AvailabilityResponseDto> Update(int id, AvailabilityUpdateDto dto);
        Task<bool> Delete(int id);
    }
}
