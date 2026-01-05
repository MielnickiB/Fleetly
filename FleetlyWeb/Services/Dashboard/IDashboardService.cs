using Fleetly.Shared.Dto.DashboardDtos;

namespace FleetlyWeb.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<ApiResponse<DashboardStatsDto?>> GetStatsAsync();
    }
}
