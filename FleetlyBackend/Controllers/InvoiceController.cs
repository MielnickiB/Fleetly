using Fleetly.Shared.Dto.InvoiceDtos;
using FleetlyBackend.Services.InvoiceService;
using FleetlyBackend.Services.PaymentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController(IInvoiceService service, IConfiguration config) : ControllerBase
    {
        private readonly IInvoiceService _service = service;
        private readonly IConfiguration _config = config;

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

        [HttpPost("{id:int}/pay-online")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<PaymentInitResponseDto>> InitPayment(int id, [FromServices] PaymentService paymentService)
        {
            try
            {
                var clientUrl = _config.GetValue<string>("AppSettings:ClientUrl") ?? "http://localhost:5251";

                var domain = $"{clientUrl}/invoices";

                return Ok(await paymentService.CreateCheckoutSession(id, domain));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("confirm-payment")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult> ConfirmPayment([FromQuery] string sessionId, [FromServices] PaymentService paymentService)
        {
            try
            {
                var isPaid = await paymentService.VerifySessionPayment(sessionId);

                if (!isPaid)
                {
                    return BadRequest("Płatność nie została jeszcze potwierdzona przez Stripe.");
                }

                var method = await paymentService.GetSessionPaymentMethod(sessionId);

                await _service.ConfirmPayment(sessionId, method);

                return Ok(true);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
