using Fleetly.Shared.Dto.BrandModelDtos;
using FleetlyBackend.Services.BrandModelService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandModelController(IBrandModelService service) : ControllerBase
    {
        private readonly IBrandModelService _service = service;

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<BrandModelResponseDto>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<BrandModelResponseDto>> Get(int id)
        {
            var result = await _service.Get(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BrandModelResponseDto>> Create(BrandModelCreateDto dto)
        {
            try { return Ok(await _service.Create(dto)); }
            catch (InvalidOperationException ex)
            { return BadRequest(ex.Message); }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BrandModelResponseDto>> Update(int id, BrandModelUpdateDto dto)
        {
            try { return Ok(await _service.Update(id, dto)); }
            catch (InvalidOperationException ex)
            { return BadRequest(ex.Message); }
            catch (ArgumentException ex)
            { return NotFound(ex.Message); }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try { return Ok(await _service.Deactivate(id)); }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }
    }
}
