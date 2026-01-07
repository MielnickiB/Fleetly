using Fleetly.Shared.Dto.DashboardDtos;
using Fleetly.Shared.Client;

namespace FleetlyMobile.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<ApiResponse<DriverDashboardDto?>> GetDashboardDataAsync();
    }
}
