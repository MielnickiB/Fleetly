
using Fleetly.Shared.Dto.VehicleDtos;
using FleetlyBackend.Helpers;
using FleetlyBackend.Services.VehicleService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController(IVehicleService service) : ControllerBase
    {
        private readonly IVehicleService _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<VehicleResponseDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _service.GetAll(page, pageSize));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin, Client")]
        public async Task<ActionResult<VehicleResponseDto>> Get(int id)
        {
            try
            {
                var v = await _service.GetById(id);
                return v is null ? NotFound("Nie znaleziono danego pojazdu") : Ok(v);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Client")]
        public async Task<ActionResult<VehicleResponseDto>> Create(VehicleCreateDto dto)
        {
            try
            {
                return Ok(await _service.Create(dto));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin, Client")]
        public async Task<ActionResult<VehicleResponseDto>> Update(int id, VehicleUpdateDto dto)
        {
            try
            {
                return Ok(await _service.Update(id, dto));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin, Client")]
        public async Task<ActionResult<bool>> Deactivate(int id)
        {
            try
            {
                return Ok(await _service.Deactivate(id));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
