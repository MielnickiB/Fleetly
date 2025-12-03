using Fleetly.Shared.Dto.CarBrandDtos;
using FleetlyBackend.Services.CarBrandService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CarBrandController(ICarBrandService service) : ControllerBase
    {
        private readonly ICarBrandService _service = service;

        [HttpGet]
        public async Task<ActionResult<List<CarBrandResponseDto>>> GetAll(int page = 1, int pageSize = 20)
        {
            return Ok(await _service.GetAll(page, pageSize));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CarBrandResponseDto>> Get(int id)
        {
            var brand = await _service.GetById(id);
            return brand is null ? NotFound() : Ok(brand);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CarBrandResponseDto>> Create(CarBrandCreateDto dto)
        {
            try
            {
                var brand = await _service.Create(dto);
                return CreatedAtAction(nameof(Get), new { id = brand.Id }, brand);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CarBrandResponseDto>> Update(int id, CarBrandUpdateDto dto)
        {
            try
            {
                return Ok(await _service.Update(id, dto));
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

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try { return Ok(await _service.Deactivate(id)); }
            catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }
    }
}
