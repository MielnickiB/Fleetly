using Fleetly.Shared.Dto.InvoiceDtos;
using FleetlyBackend.Helpers;
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<InvoiceResponseDto>>> GetAll(int page = 1, int pageSize = 10)
            => Ok(await _service.GetAll(page, pageSize));

        [HttpGet("client/{clientId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<InvoiceResponseDto>>> GetForClient(int clientId)
        {
            try
            {
                var result = await _service.GetForClient(clientId);
                return Ok(result);
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

        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<List<InvoiceResponseDto>>> GetMyInvoices()
        {
            if (!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);
            try
            {
                var result = await _service.GetForClient(userId);
                return Ok(result);
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

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InvoiceResponseDto>> GetById(int id)
        {
            var result = await _service.GetById(id);
            return result is null ? NotFound(new { error = "Podana faktura nie istnieje" }) : Ok(result);
        }

        [HttpGet("my/{id:int}")]
        [Authorize]
        public async Task<ActionResult<InvoiceResponseDto>> GetMyById(int id)
        {
            if (!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);
            var result = await _service.GetById(id);
            return result is null ? NotFound(new { error = "Podana faktura nie istnieje" }) : Ok(result);
        }

        [HttpPost("generate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InvoiceResponseDto>> Create(InvoiceCreateDto dto)
        {
            try { return Ok(await _service.Create(dto)); }
            catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InvoiceResponseDto>> Update(int id, InvoiceUpdateDto dto)
        {
            try { return Ok(await _service.Update(id, dto)); }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                return Ok(await _service.Delete(id));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
