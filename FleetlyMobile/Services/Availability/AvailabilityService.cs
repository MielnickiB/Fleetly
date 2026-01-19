using Fleetly.Shared.Client;
using Fleetly.Shared.Dto.AvailabilityDtos;

namespace FleetlyMobile.Services.Availability
{
    public class AvailabilityService(ApiClient api) : IAvailabilityService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/Availability";

        public async Task<ApiResponse<List<AvailabilityResponseDto>?>> GetByRangeAsync(DateOnly start, DateOnly end)
        {
            var queryString = $"?start={start:yyyy-MM-dd}&end={end:yyyy-MM-dd}";
            return await _api.GetAsync<List<AvailabilityResponseDto>>($"{BaseUrl}{queryString}");
        }

        public async Task<ApiResponse<AvailabilityResponseDto?>> GetAsync(int id)
        {
            return await _api.GetAsync<AvailabilityResponseDto>($"{BaseUrl}/{id}");
        }

        public async Task<ApiResponse<List<AvailabilityResponseDto>?>> CreateAsync(AvailabilityCreateDto dto)
        {
            return await _api.PostAsync<AvailabilityCreateDto, List<AvailabilityResponseDto>>(BaseUrl, dto);
        }

        public async Task<ApiResponse<AvailabilityResponseDto?>> UpdateAsync(int id, AvailabilityUpdateDto dto)
        {
            return await _api.PutAsync<AvailabilityUpdateDto, AvailabilityResponseDto>($"{BaseUrl}/{id}", dto);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            return await _api.DeleteAsync($"{BaseUrl}/{id}");
        }

        public async Task<ApiResponse<bool>> DeleteRangeAsync(DateOnly start, DateOnly end)
        {
            var queryString = $"?start={start:yyyy-MM-dd}&end={end:yyyy-MM-dd}";
            return await _api.DeleteAsync($"{BaseUrl}/range{queryString}");
        }
    }
}