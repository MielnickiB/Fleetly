using Fleetly.Shared.Dto.AvailabilityDtos;

namespace FleetlyBackend.Services.AvailabilityService
{
    public interface IAvailabilityService
    {
        Task<List<AvailabilityResponseDto>> GetByRange(DateOnly start, DateOnly end);
        Task<AvailabilityResponseDto?> Get(int id);
        Task<List<AvailabilityResponseDto>> Create(AvailabilityCreateDto dto);
        Task<AvailabilityResponseDto> Update(int id, AvailabilityUpdateDto dto);
        Task Delete(int id);
        Task DeleteByRange(DateOnly start, DateOnly end);
    }
}
