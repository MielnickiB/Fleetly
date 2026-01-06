using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.VehicleDtos;
using Fleetly.Shared.Client;

namespace FleetlyWeb.Services.Vehicles
{
    public class VehicleService(ApiClient api) : IVehicleService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/Vehicle";
        public async Task<ApiResponse<PagedResult<VehicleResponseDto>>> GetAllVehiclesAsync()
        {

            var resp = await _api.GetAsync<PagedResult<VehicleResponseDto>>(BaseUrl) 
                ?? throw new Exception("Brak odpowiedzi z serwera.");

            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się pobrać pojazdów.");
            }

            return resp!;
        }

        public async Task<ApiResponse<VehicleResponseDto?>> GetVehicleByIdAsync(int vehicleId)
        {
            var resp = await _api.GetAsync<VehicleResponseDto?>($"{BaseUrl}/{vehicleId}")
                ?? throw new Exception("Brak odpowiedzi z serwera.");

            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się pobrać pojazdu.");
            }

            return resp;
        }

        public async Task<ApiResponse<VehicleResponseDto>> CreateVehicleAsync(VehicleCreateDto dto)
        {
            var resp = await _api.PostAsync<VehicleCreateDto, VehicleResponseDto>(BaseUrl, dto)
                ?? throw new Exception("Brak odpowiedzi z serwera.");

            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się utworzyć pojazdu.");
            }

            return resp!;
        }

        public async Task<ApiResponse<VehicleResponseDto>> UpdateVehicleAsync(int id, VehicleUpdateDto dto)
        {
            var resp = await _api.PutAsync<VehicleUpdateDto, VehicleResponseDto>($"{BaseUrl}/{id}", dto)
                ?? throw new Exception("Brak odpowiedzi z serwera.");

            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się zaktualizować pojazdu.");
            }

            return resp!;
        }

        public async Task<ApiResponse<bool>> DeactivateVehicleAsync(int id)
        {
            var resp = await _api.DeleteAsync($"{BaseUrl}/{id}")
                ?? throw new Exception("Brak odpowiedzi z serwera.");
            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się dezaktywować pojazdu.");
            }
            return resp;
        }
    }
}
