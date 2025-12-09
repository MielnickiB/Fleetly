using Fleetly.Shared.Dto.CostLimitDtos;
using FleetlyBackend.Services.CostLimitService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CostLimitController(ICostLimitService service) : ControllerBase
    {
        private readonly ICostLimitService _service = service;

        [HttpGet]
        public async Task<ActionResult<List<CostLimitResponseDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            => Ok(await _service.GetAll(page, pageSize));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CostLimitResponseDto>> Get(int id)
        {
            var result = await _service.Get(id);
            return result is null ? NotFound("Nie znaleziono danego kosztorysu") : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CostLimitResponseDto>> Create(CostLimitCreateDto dto)
        {
            try { return Ok(await _service.Create(dto)); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CostLimitResponseDto>> Update(int id, CostLimitUpdateDto dto)
        {
            try { return Ok(await _service.Update(id, dto)); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
            catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try { return Ok(await _service.Delete(id)); }
            catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        }
    }
}
