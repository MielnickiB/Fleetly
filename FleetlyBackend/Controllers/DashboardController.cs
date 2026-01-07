using Fleetly.Shared.Dto.DashboardDtos;
using FleetlyBackend.Services.DashboardService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(IDashboardService service) : ControllerBase
    {
        private readonly IDashboardService _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin, Client")]
        public async Task<ActionResult<DashboardStatsDto>> GetStats()
        {
            var stats = await _service.GetStatsAsync();
            return Ok(stats);
        }

        [HttpGet("worker")]
        [Authorize(Roles = "Worker")]
        public async Task<ActionResult<DriverDashboardDto>> GetDriverDashboard()
        {
            var result = await _service.GetDriverDashboardAsync();
            return Ok(result);
        }
    }
}
