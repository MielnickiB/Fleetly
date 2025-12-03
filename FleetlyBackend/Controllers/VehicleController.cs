
using Fleetly.Shared.Dto.VehicleDtos;
using FleetlyBackend.Helpers;
using FleetlyBackend.Services.VehicleService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VehicleController(IVehicleService service) : ControllerBase
    {
        private readonly IVehicleService _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<VehicleResponseDto>>> GetAll(int page = 1, int pageSize = 20)
        {
            return Ok(await _service.GetAll(page, pageSize));
        }

        [HttpGet("my")]
        public async Task<ActionResult<List<VehicleResponseDto>>> GetMyVehicles()
        {
            if (!User.TryGetUserId(out var userId, out var err))
                return Unauthorized(err);

            return Ok(await _service.GetUserVehicles(userId));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VehicleResponseDto>> Get(int id)
        {
            var v = await _service.GetById(id);
            return v is null ? NotFound() : Ok(v);
        }

        [HttpPost]
        public async Task<ActionResult<VehicleResponseDto>> Create(VehicleCreateDto dto)
        {
            if (!User.TryGetUserId(out var userId, out var err))
                return Unauthorized(err);

            try
            {
                return Ok(await _service.Create(userId, dto));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VehicleResponseDto>> Update(int id, VehicleUpdateDto dto)
        {
            try { return Ok(await _service.Update(id, dto)); }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("my/{id:int}")]
        public async Task<ActionResult<VehicleResponseDto>> UpdateMe(int id, VehicleUpdateDto dto)
        {
            if (!User.TryGetUserId(out var userId, out var err))
                return Unauthorized(err);

            try { return Ok(await _service.Update(id, dto, userId)); }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> Deactivate(int id)
        {
            try { return Ok(await _service.Deactivate(id)); }
            catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpDelete("my/{id:int}")]
        public async Task<ActionResult<bool>> DeactivateMe(int id)
        {
            if (!User.TryGetUserId(out var userId, out var err))
                return Unauthorized(err);

            try { return Ok(await _service.Deactivate(id, userId)); }
            catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { error = ex.Message}); }
        }
    }
}
