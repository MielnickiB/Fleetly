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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<LocationResponseDto>>> GetAll(int page, int pageSize)
        {

            return Ok(await _service.GetAll(page, pageSize));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<LocationResponseDto>> Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Niepoprawne Id.");
            }

            var loc = await _service.GetById(id);
            return loc is null ? NotFound("Nie znaleziono danej lokalizacji.") : Ok(loc);
        }

        [HttpGet("my")]
        public async Task<ActionResult<List<LocationResponseDto>>> GetUserLocations()
        {
            if (!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);
            return Ok(await _service.GetUserLocations(userId));
        }

        [HttpPost]
        public async Task<ActionResult<LocationResponseDto>> Create(LocationCreateDto dto)
        {
            if (!User.TryGetUserId(out var userId, out var err))
                return Unauthorized(new { error = err });

                return Ok(await _service.Create(dto, userId));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<LocationResponseDto>> Update(int id, LocationUpdateDto dto)
        {
            if (id <= 0)
            {
                return BadRequest("Niepoprawne Id.");
            }

            try
            {
                return Ok(await _service.Update(id, dto));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("my/{id:int}")]
        public async Task<ActionResult<LocationResponseDto>> UpdateMe(int id, LocationUpdateDto dto)
        {
            if (!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);

            if (id <= 0)
            {
                return BadRequest("Niepoprawne Id.");
            }

            try
            {
                return Ok(await _service.Update(id, dto, userId));
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

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Niepoprawne Id.");
            }

            try
            {
                return Ok(await _service.Deactivate(id));
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

        [HttpDelete("my/{id:int}")]
        public async Task<ActionResult<LocationResponseDto>> DeleteMe(int id)
        {
            if (!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);
            if (id <= 0)
            {
                return BadRequest("Niepoprawne Id.");
            }
            try
            {
                return Ok(await _service.Deactivate(id, userId));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

    }
}
