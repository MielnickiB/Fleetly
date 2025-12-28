using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.OrderDtos;
using FleetlyBackend.Helpers;
using FleetlyBackend.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers.OrderControllers
{
    [Route("api/client/orders")]
    [ApiController]
    [Authorize(Roles = "Client")]
    public class OrdersClientController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;

        [HttpGet]
        public async Task<ActionResult<PagedResult<OrderResponseDto>>> GetAllOrders()
        {
            return Ok(await _orderService.GetAllOrders());
        }

        [HttpGet("{orderId:int}")]
        public async Task<ActionResult<OrderResponseDto>> Get(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderById(orderId);
                return order is null ? NotFound("Nie znaleziono zlecenia.") : Ok(order);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> Create([FromBody] OrderCreateDto orderCreateDto)
        {
            try
            {
                return Ok(await _orderService.CreateOrder(orderCreateDto));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{orderId:int}")]
        public async Task<ActionResult<OrderResponseDto>> Update(int orderId, [FromBody] OrderUpdateDto orderUpdateDto)
        {
            try
            {
                var result = await _orderService.UpdateOrder(orderId, orderUpdateDto);
                return Ok(result);
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{orderId:int}")]
        public async Task<ActionResult<OrderResponseDto>> Cancel(int orderId)
        {
            try
            {
                var result = await _orderService.CancelOrder(orderId);
                return Ok(result);
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }
    }
}
