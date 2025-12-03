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
        public async Task<IActionResult> GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);
            return Ok(await _orderService.GetAllOrdersForClient(userId, page, pageSize));
        }

        [HttpGet("{orderId:int}/details")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            if (!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);

            try
            {
                var dto = await _orderService.GetOrderForClientById(orderId, userId);
                return Ok(dto);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderCreateDto)
        {
            if (!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);
            try
            {
                return Ok(await _orderService.CreateOrder(userId, orderCreateDto));
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

        [HttpPut("{orderId:int}/update")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] OrderUpdateDto orderUpdateDto)
        {
            if (!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);
            try
            {
                var result = await _orderService.UpdateOrder(orderId, orderUpdateDto, userId);
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
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpDelete("{orderId:int}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            if (!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);
            try
            {
                var result = await _orderService.CancelOrder(orderId, userId);
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
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}
