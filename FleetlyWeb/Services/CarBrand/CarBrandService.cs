using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.CarBrandDtos;

namespace FleetlyWeb.Services.CarBrand
{
    public class CarBrandService(ApiClient api) : ICarBrandService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/CarBrand";

        public async Task<ApiResponse<PagedResult<CarBrandResponseDto>>> GetAllCarBrandsAsync()
        {
            var resp = await _api.GetAsync<PagedResult<CarBrandResponseDto>>(BaseUrl)
                ?? throw new Exception("Brak odpowiedzi z serwera.");

            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się pobrać marek samochodów.");
            }

            return resp!;
        }
    }
}