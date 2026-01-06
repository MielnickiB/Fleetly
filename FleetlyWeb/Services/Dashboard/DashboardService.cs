using Fleetly.Shared.Dto.DashboardDtos;
using Fleetly.Shared.Client;

namespace FleetlyWeb.Services.Dashboard
{
    public class DashboardService(ApiClient api) : IDashboardService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/Dashboard";

        public async Task<ApiResponse<DashboardStatsDto?>> GetStatsAsync()
        {
            return await _api.GetAsync<DashboardStatsDto>(BaseUrl);
        }
    }
}
