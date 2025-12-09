using Fleetly.Shared.Dto.InvoiceDtos;
using FleetlyBackend.Services.InvoiceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController(IInvoiceService service) : ControllerBase
    {
        private readonly IInvoiceService _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin, Client")]
        public async Task<ActionResult<List<InvoiceResponseDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            => Ok(await _service.GetAll(page, pageSize));


        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin, Client")]
        public async Task<ActionResult<InvoiceResponseDto>> GetById(int id)
        {
            try
            {
                var result = await _service.GetById(id);
                return result is null ? NotFound("Podana faktura nie istnieje.") : Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InvoiceResponseDto>> Create(InvoiceCreateDto dto)
        {
            try { return Ok(await _service.Create(dto)); }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InvoiceResponseDto>> Update(int id, InvoiceUpdateDto dto)
        {
            try { return Ok(await _service.Update(id, dto)); }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try { return Ok(await _service.Delete(id)); }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
        }
    }
}
