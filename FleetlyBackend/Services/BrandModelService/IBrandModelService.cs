using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.BrandModelDtos;

namespace FleetlyBackend.Services.BrandModelService
{
    public interface IBrandModelService
    {
        Task<PagedResult<BrandModelResponseDto>> GetAll();
        Task<BrandModelResponseDto?> Get(int id);
        Task<BrandModelResponseDto> Create(BrandModelCreateDto dto);
        Task<BrandModelResponseDto> Update(int id, BrandModelUpdateDto dto);
        Task<bool> Deactivate(int id);
    }
}
