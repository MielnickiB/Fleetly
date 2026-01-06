using Fleetly.Shared.Dto.DashboardDtos;
using Fleetly.Shared.Client;

namespace FleetlyWeb.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<ApiResponse<DashboardStatsDto?>> GetStatsAsync();
    }
}
