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
        public async Task<ActionResult<List<InvoiceResponseDto>>> GetAll(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] bool showPaid = false,
            [FromQuery] string? search = null)
            => Ok(await _service.GetAll(page, pageSize, showPaid, search));


        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin, Client")]
        public async Task<ActionResult<InvoiceResponseDto>> GetById(int id)
        {
            try
            {
                var result = await _service.GetById(id);
                return result is null ? NotFound("Faktura nie istnieje.") : Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(InvoiceCreateDto dto)
        {
            try 
            { 
                await _service.Create(dto);
                return Ok(true); 
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

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int id, InvoiceUpdateDto dto)
        {
            try 
            {
                await _service.Update(id, dto);
                return Ok(true); 
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
        }

        [HttpPost("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id, InvoicePayDto dto)
        {
            try 
            { 
                await _service.Pay(id, dto);
                return Ok(true); 
            }
            catch (ArgumentException ex) 
            { 
                return NotFound(ex.Message); 
            }
        }
    }
}
