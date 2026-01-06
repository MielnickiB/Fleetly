using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.CostLimitDtos;
using Fleetly.Shared.Client;

namespace FleetlyWeb.Services.CostLimits
{
    public class CostLimitService(ApiClient api) : ICostLimitService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/CostLimit";

        public async Task<ApiResponse<PagedResult<CostLimitResponseDto>?>> GetAllAsync()
        {
            return await _api.GetAsync<PagedResult<CostLimitResponseDto>>(BaseUrl);
        }

        public async Task<ApiResponse<CostLimitResponseDto?>> GetAsync(int id)
        {
            return await _api.GetAsync<CostLimitResponseDto>($"{BaseUrl}/{id}");
        }

        public async Task<ApiResponse<CostLimitResponseDto?>> CreateAsync(CostLimitCreateDto dto)
        {
            return await _api.PostAsync<CostLimitCreateDto, CostLimitResponseDto>(BaseUrl, dto);
        }

        public async Task<ApiResponse<CostLimitResponseDto?>> UpdateAsync(int id, CostLimitUpdateDto dto)
        {
            return await _api.PutAsync<CostLimitUpdateDto, CostLimitResponseDto>($"{BaseUrl}/{id}", dto);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            return await _api.DeleteAsync<bool>($"{BaseUrl}/{id}");
        }
    }
}
