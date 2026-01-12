using Fleetly.Shared.Dto.ProtocolDtos;
using FleetlyBackend.Services.ProtocolService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Worker")]
    public class ProtocolController(IProtocolService protocolService) : ControllerBase
    {
        private readonly IProtocolService _service = protocolService;

        [HttpPost("start")]
        public async Task<ActionResult<int>> StartProtocol([FromBody] ProtocolInitDto dto)
        {
            try
            {
                var protocolId = await _service.StartProtocolAsync(dto);
                return Ok(protocolId);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{id}/photos")]
        public async Task<IActionResult> AddPhoto(int id, [FromForm] ProtocolPhotoDto dto)
        {
            if (id != dto.ProtocolId) return BadRequest("ID protokołu się nie zgadza.");

            try
            {
                await _service.AddProtocolPhotoAsync(dto);
                return Ok();
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPost("{id}/damages")]
        public async Task<IActionResult> AddDamage(int id, [FromForm] DamageCreateDto dto)
        {
            dto.ProtocolId = id;

            try
            {
                await _service.AddDamageAsync(dto);
                return Ok();
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPost("{id}/finish")]
        public async Task<IActionResult> FinishProtocol(int id, [FromForm] ProtocolFinishDto dto)
        {
            if (id != dto.ProtocolId) return BadRequest("ID protokołu się nie zgadza.");

            try
            {
                await _service.FinishProtocolAsync(dto);
                return Ok();
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }
    }
}