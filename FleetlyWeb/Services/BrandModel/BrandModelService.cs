using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.BrandModelDtos;

namespace FleetlyWeb.Services.BrandModel
{
    public class BrandModelService(ApiClient api) : IBrandModelService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/BrandModel";
        public async Task<ApiResponse<PagedResult<BrandModelResponseDto>>> GetAllBrandModelsAsync()
        {
            var resp = await _api.GetAsync<PagedResult<BrandModelResponseDto>>(BaseUrl)
                ?? throw new Exception("Brak odpowiedzi z serwera.");
            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się pobrać modeli marek.");
            }
            return resp!;
        }
    }
}
