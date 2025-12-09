using Fleetly.Shared.Dto.CarBrandDtos;

namespace FleetlyBackend.Services.CarBrandService
{
    public interface ICarBrandService
    {
        public Task<List<CarBrandResponseDto>> GetAll(int page = 1, int pageSize = 10);
        public Task<CarBrandResponseDto?> GetById(int id);
        public Task<CarBrandResponseDto> Create(CarBrandCreateDto carBrandCreateDto);
        public Task<CarBrandResponseDto> Update(int id, CarBrandUpdateDto carBrandUpdateDto);
        public Task<bool> Deactivate(int id);
    }
}
