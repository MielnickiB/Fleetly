using Fleetly.Shared.Dto.NotificationDtos;
using FleetlyBackend.Helpers;
using FleetlyBackend.Services.NotificationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(INotificationService service) : ControllerBase
    {
        private readonly INotificationService _service = service;

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<NotificationResponseDto>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NotificationResponseDto>> Create(NotificationCreateDto dto)
        {
            try { return Ok(await _service.Create(dto)); }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
        }

        [HttpPut("read/{id:int}")]
        [Authorize]
        public async Task<ActionResult<bool>> MarkAsRead(int id)
        {
            try { return Ok(await _service.MarkAsRead(id)); }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try { return Ok(await _service.Delete(id)); }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }
    }
}
