using Fleetly.Shared.Dto.AvailabilityDtos;
using FleetlyBackend.Services.AvailabilityService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController(IAvailabilityService service) : ControllerBase
    {
        private readonly IAvailabilityService _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin, Worker")]
        public async Task<ActionResult<List<AvailabilityResponseDto>>> GetByRange([FromQuery] DateOnly start, [FromQuery] DateOnly end)
        {
            return Ok(await _service.GetByRange(start, end));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin, Worker")]
        public async Task<ActionResult<AvailabilityResponseDto>> Get(int id)
        {
            try
            {
                var result = await _service.Get(id);
                return result is null ? NotFound() : Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Worker")]
        public async Task<ActionResult<List<AvailabilityResponseDto>>> Create(AvailabilityCreateDto dto)
        {
            try
            {
                return Ok(await _service.Create(dto));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Worker")]
        public async Task<ActionResult<AvailabilityResponseDto>> Update(int id, AvailabilityUpdateDto dto)
        {
            try
            {
                return Ok(await _service.Update(id, dto));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Worker")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _service.Delete(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
