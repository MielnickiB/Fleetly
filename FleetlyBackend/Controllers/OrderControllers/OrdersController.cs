using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.OrderDtos;
using FleetlyBackend.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers.OrderControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _service = orderService;

        [Authorize(Roles = "Admin, Client")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<OrderResponseDto>>> GetAll()
        {
            return Ok(await _service.GetAllOrders());
        }

        [Authorize(Roles = "Admin, Client")]
        [HttpGet("{orderId:int}")]
        public async Task<ActionResult<OrderResponseDto>> Get(int orderId)
        {
            try
            {
                var order = await _service.GetOrderById(orderId);
                return order is null ? NotFound("Nie odnaleziono zlecenia.") : Ok(order);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [Authorize(Roles = "Client")]
        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> Create([FromBody] OrderCreateDto orderCreateDto)
        {
            try
            {
                return Ok(await _service.CreateOrder(orderCreateDto));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        [Authorize(Roles = "Admin, Client")]
        [HttpPut("{orderId:int}")]
        public async Task<ActionResult<OrderResponseDto>> Update(int orderId, [FromBody] OrderUpdateDto dto)
        {
            try
            {
                var result = await _service.UpdateOrder(orderId, dto);
                return Ok(result);
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [Authorize(Roles = "Admin, Client")]
        [HttpDelete("{orderId:int}")]
        public async Task<ActionResult<OrderResponseDto>> Cancel(int orderId)
        {
            try
            {
                var result = await _service.CancelOrder(orderId);
                return Ok(result);
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{orderId:int}/costs/approve")]
        public async Task<ActionResult<OrderResponseDto>> ApproveCosts(int orderId)
        {
            try { return Ok(await _service.ApproveCostsAndCompleteOrder(orderId)); }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{orderId:int}/activate")]
        public async Task<ActionResult<OrderResponseDto>> Approve(int orderId)
        {
            try { return Ok(await _service.ApproveOrder(orderId)); }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }
    }
}
