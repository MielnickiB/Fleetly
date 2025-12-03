using Fleetly.Shared.Dto.NotificationDtos;
using FleetlyBackend.Helpers;
using FleetlyBackend.Services.NotificationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController(INotificationService service) : ControllerBase
    {
        private readonly INotificationService _service = service;

        [HttpGet("my")]
        public async Task<ActionResult<List<NotificationResponseDto>>> GetMy()
        {
            if (!User.TryGetUserId(out var userId, out var err))
                return Unauthorized(err);

            return Ok(await _service.GetForUser(userId));
        }

        [HttpGet("my/{id:int}")]
        public async Task<ActionResult<NotificationResponseDto>> GetMy(int id)
        {
            if (!User.TryGetUserId(out var userId, out var err))
                return Unauthorized(err);

            var notif = await _service.Get(id, userId);
            return notif is null ? NotFound(new {error = "Nie znaleziono danej notyfikacji"}) : Ok(notif);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NotificationResponseDto>> Create(NotificationCreateDto dto)
        {
            try { return Ok(await _service.Create(dto)); }
            catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        }

        [HttpPut("my/{id:int}/read")]
        public async Task<ActionResult<bool>> MarkAsRead(int id)
        {
            if (!User.TryGetUserId(out var userId, out var err))
                return Unauthorized(err);

            try { return Ok(await _service.MarkAsRead(id, userId)); }
            catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        }

        [HttpDelete("my/{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            if (!User.TryGetUserId(out var userId, out var err))
                return Unauthorized(err);

            try { return Ok(await _service.Delete(id, userId)); }
            catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        }
    }
}
