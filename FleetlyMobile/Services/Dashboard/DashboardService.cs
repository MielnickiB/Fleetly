using Fleetly.Shared.Client;
using Fleetly.Shared.Dto.DashboardDtos;

namespace FleetlyMobile.Services.Dashboard
{
    public class DashboardService(ApiClient api) : IDashboardService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/Dashboard";
        public async Task<ApiResponse<DriverDashboardDto?>> GetDashboardDataAsync()
        {
            return await _api.GetAsync<DriverDashboardDto>($"{BaseUrl}/worker");
        }
    }
}
