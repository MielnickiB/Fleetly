using Fleetly.Shared.Dto.AvailabilityDtos;
using FleetlyBackend.Helpers;
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<AvailabilityResponseDto>>> GetAll(int page = 1, int pageSize = 10)
        {
            return Ok(await _service.GetAll(page, pageSize));
        }

        [HttpGet("worker/{workerId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<AvailabilityResponseDto>>> GetByWorker(int workerId)
        {
            try
            {
                return Ok(await _service.GetWorkerAvailability(workerId));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("my")]
        [Authorize(Roles = "Worker")]
        public async Task<ActionResult<List<AvailabilityResponseDto>>> GetMine()
        {
            if (!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);

            return Ok(await _service.GetWorkerAvailability(userId));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Worker")]
        public async Task<ActionResult<AvailabilityResponseDto>> Get(int id)
        {
            var result = await _service.Get(id);
            return result is null ? NotFound(new { error = "Nie znaleziono danej dostępności" }) : Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Worker")]
        public async Task<ActionResult<AvailabilityResponseDto>> Create(AvailabilityCreateDto dto)
        {
            if (!User.TryGetUserId(out var userId, out var err))
                return Unauthorized(new { error = err });

            try { return Ok(await _service.Create(userId, dto)); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
            catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Worker")]
        public async Task<ActionResult<AvailabilityResponseDto>> Update(int id, AvailabilityUpdateDto dto)
        {
            try { return Ok(await _service.Update(id, dto)); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
            catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Worker")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                return Ok(await _service.Delete(id));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
