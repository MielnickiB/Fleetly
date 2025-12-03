using Fleetly.Shared.Dto.BrandModelDtos;

namespace FleetlyBackend.Services.BrandModelService
{
    public interface IBrandModelService
    {
        Task<List<BrandModelResponseDto>> GetAll(int page = 1, int pageSize = 20);
        Task<BrandModelResponseDto?> Get(int id);
        Task<BrandModelResponseDto> Create(BrandModelCreateDto dto);
        Task<BrandModelResponseDto> Update(int id, BrandModelUpdateDto dto);
        Task<bool> Deactivate(int id);
    }
}
