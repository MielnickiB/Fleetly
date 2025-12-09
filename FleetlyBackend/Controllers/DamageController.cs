using Fleetly.Shared.Dto.DamageDtos;
using FleetlyBackend.Services.DamageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DamageController(IDamageService service) : ControllerBase
    {
        private readonly IDamageService _service = service;

        [HttpGet("vehicle/{vehicleId:int}")]
        public async Task<ActionResult<List<DamageResponseDto>>> GetForVehicle(int vehicleId)
        {
            try
            {
                return Ok(await _service.GetForVehicle(vehicleId));
            }
            catch (ArgumentException ex)
            {
                return NotFound( ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DamageResponseDto>> Get(int id)
        {
            var d = await _service.Get(id);
            return d is null ? NotFound("Nie znaleziono danego uszkodzenia.") : Ok(d);
        }

        [HttpPost]
        public async Task<ActionResult<DamageResponseDto>> Create([FromForm] DamageCreateDto dto)
        {
            try { return Ok(await _service.Create(dto)); }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<DamageResponseDto>> Update(int id, [FromForm] DamageUpdateDto dto)
        {
            try { return Ok(await _service.Update(id, dto)); }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                return Ok(await _service.Delete(id));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}