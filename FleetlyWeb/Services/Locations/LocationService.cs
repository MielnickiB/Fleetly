using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.LocationDtos;

namespace FleetlyWeb.Services.Locations
{
    public class LocationService(ApiClient api) : ILocationService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/Location";

        public async Task<ApiResponse<PagedResult<LocationResponseDto>>> GetAllLocationsAsync()
        {
            var resp = await _api.GetAsync<PagedResult<LocationResponseDto>>(BaseUrl) 
                ?? throw new Exception("Brak odpowiedzi z serwera.");
            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się pobrać lokalizacji.");
            }
            return resp!;
        }
        public async Task<ApiResponse<LocationResponseDto?>> GetLocationByIdAsync(int locationId)
        {
            var resp = await _api.GetAsync<LocationResponseDto?>($"{BaseUrl}/{locationId}")
                ?? throw new Exception("Brak odpowiedzi z serwera.");
            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się pobrać lokalizacji.");
            }
            return resp;
        }

        public async Task<ApiResponse<LocationResponseDto>> CreateLocationAsync(LocationCreateDto dto)
        {
            var resp = await _api.PostAsync<LocationCreateDto, LocationResponseDto>(BaseUrl, dto)
                ?? throw new Exception("Brak odpowiedzi z serwera.");
            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się utworzyć lokalizacji.");
            }
            return resp!;
        }

        public async Task<ApiResponse<LocationResponseDto>> UpdateLocationAsync(int id, LocationUpdateDto dto)
        {
            var resp = await _api.PutAsync<LocationUpdateDto, LocationResponseDto>($"{BaseUrl}/{id}", dto)
                ?? throw new Exception("Brak odpowiedzi z serwera.");
            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się zaktualizować lokalizacji.");
            }
            return resp!;
        }

        public async Task<ApiResponse<bool>> DeactivateLocationAsync(int id)
        {
            var resp = await _api.DeleteAsync($"{BaseUrl}/{id}")
                ?? throw new Exception("Brak odpowiedzi z serwera.");
            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się dezaktywować lokalizacji.");
            }
            return resp;
        }
    }
}
