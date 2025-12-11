using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.VehicleDtos;
using Microsoft.AspNetCore.WebUtilities;

namespace FleetlyWeb.Services
{
    public class VehicleService(ApiClient api) : IVehicleService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/Vehicle";
        public async Task<ApiResponse<PagedResult<VehicleResponseDto>>> GetAllVehiclesAsync(VehicleFilterQuery query)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["page"] = query.Page.ToString(),
                ["pageSize"] = query.PageSize.ToString(),
                ["searchTerm"] = query.SearchTerm,
                ["sortBy"] = query.SortBy,
                ["sortDirection"] = query.SortDirection,
                ["userFullName"] = query.UserFullName,
                ["brandName"] = query.BrandName,
                ["modelName"] = query.ModelName,
                ["fuelType"] = query.FuelType?.ToString(),
                ["isActive"] = query.IsActive?.ToString().ToLowerInvariant(),
            };

            var filteredParams = queryParams
                .Where(p => !string.IsNullOrEmpty(p.Value))
                .ToDictionary(p => p.Key, p => p.Value);

            var url = QueryHelpers.AddQueryString(BaseUrl, filteredParams);

            var resp = await _api.GetAsync<PagedResult<VehicleResponseDto>>(url) 
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
            return await _api.DeleteAsync($"{BaseUrl}/{id}");
        }
    }
}
