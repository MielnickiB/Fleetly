using Fleetly.Shared.Dto.DashboardDtos;

namespace FleetlyBackend.Services.DashboardService
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetStatsAsync();
    }
}
