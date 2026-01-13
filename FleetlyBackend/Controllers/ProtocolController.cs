using Fleetly.Shared.Dto.DamageDtos;
using Fleetly.Shared.Dto.ProtocolDtos;
using FleetlyBackend.Services.ProtocolService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProtocolController(IProtocolService protocolService) : ControllerBase
    {
        private readonly IProtocolService _service = protocolService;

        [HttpGet("order/{orderId:int}")]
        public async Task<ActionResult<ProtocolResponseDto>> GetByOrder(int orderId)
        {
            try
            {
                var result = await _service.GetProtocolByOrderIdAsync(orderId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPost("start")]
        public async Task<ActionResult<ProtocolResponseDto>> StartProtocol([FromBody] ProtocolInitDto dto)
        {
            try
            {
                var result = await _service.StartProtocolAsync(dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{id:int}/photos")]
        public async Task<ActionResult<ProtocolResponseDto>> AddPhoto(int id, [FromForm] ProtocolPhotoDto dto)
        {
            if (id != dto.ProtocolId) return BadRequest("ID protokołu się nie zgadza.");

            try
            {
                var result = await _service.AddProtocolPhotoAsync(dto);
                return Ok(result);
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPost("{id:int}/damages")]
        public async Task<ActionResult<ProtocolResponseDto>> AddDamage(int id, [FromForm] DamageCreateDto dto)
        {
            dto.ProtocolId = id;

            try
            {
                var result = await _service.AddDamageAsync(dto);
                return Ok(result);
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpDelete("damages/{damageId:int}")]
        public async Task<ActionResult<ProtocolResponseDto>> DeleteDamage(int damageId)
        {
            try
            {
                var result = await _service.DeleteDamageAsync(damageId);
                return Ok(result);
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPatch("{protocolId:int}/damages/{damageId:int}/fix")]
        public async Task<ActionResult<ProtocolResponseDto>> MarkDamageFixed(int protocolId, int damageId)
        {
            try
            {
                var result = await _service.MarkDamageAsFixedAsync(damageId, protocolId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPost("{id:int}/finish")]
        public async Task<ActionResult<ProtocolResponseDto>> FinishProtocol(int id, [FromForm] ProtocolFinishDto dto)
        {
            if (id != dto.ProtocolId) return BadRequest("ID protokołu się nie zgadza.");

            try
            {
                var result = await _service.FinishProtocolAsync(dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPatch("{id:int}/step")]
        public async Task<ActionResult> UpdateStep(int id, [FromBody] int step)
        {
            try
            {
                await _service.UpdateStepAsync(id, step);
                return Ok(true);
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }
    }
}