using Fleetly.Shared.Dto.NotificationDtos;
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

        [HttpGet]
        public async Task<ActionResult<List<NotificationResponseDto>>> GetMyNotifications()
        {
            return Ok(await _service.GetMyNotifications());
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            return Ok(await _service.GetUnreadCount());
        }

        [HttpPut("{id:int}/read")]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            try
            {
                await _service.MarkAsRead(id);
                return Ok(true);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("read-all")]
        public async Task<ActionResult> MarkAllAsRead()
        {
            await _service.MarkAllAsRead();
            return Ok(true);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _service.Delete(id);
                return Ok(true);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
