using FleetlyBackend.Services.LocationService;
using FleetlyBackend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fleetly.Shared.Dto.LocationDtos;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LocationController(ILocationService service) : ControllerBase
    {
        private readonly ILocationService _service = service;

        [HttpGet]
        public async Task<ActionResult<List<LocationResponseDto>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LocationResponseDto>> Get(int id)
        {
            try
            {
                var loc = await _service.GetById(id);
                return loc is null ? NotFound("Nie znaleziono danej lokalizacji.") : Ok(loc);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
        [HttpPost]
        public async Task<ActionResult<LocationResponseDto>> Create(LocationCreateDto dto)
        {
            return Ok(await _service.Create(dto));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<LocationResponseDto>> Update(int id, LocationUpdateDto dto)
        {
            try
            {
                return Ok(await _service.Update(id, dto));
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

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
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
