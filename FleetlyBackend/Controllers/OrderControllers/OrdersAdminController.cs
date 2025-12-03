using Fleetly.Shared.Dto.OrderDtos;
using FleetlyBackend.Models;
using FleetlyBackend.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers.OrderControllers;

[Route("api/admin/orders")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminOrderController(IOrderService service) : ControllerBase
{
    private readonly IOrderService _service = service;

    [HttpGet]
    public async Task<ActionResult<List<OrderResponseDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await _service.GetAllOrders(page, pageSize));
    }

    [HttpGet("{orderId:int}")]
    public async Task<ActionResult<OrderResponseDto>> Get(int orderId)
    {
        var order = await _service.GetOrderById(orderId);
        return order is null ? NotFound(new { error = "Nie odnaleziono zlecenia." }) : Ok(order);
    }

    [HttpPost("{orderId:int}/accept")]
    public async Task<ActionResult<OrderResponseDto>> Approve(int orderId)
    {
        try { return Ok(await _service.ApproveOrder(orderId)); }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("{orderId:int}/update")]
    public async Task<ActionResult<OrderResponseDto>> Update(int orderId, [FromBody] OrderUpdateDto orderUpdateDto)
    {
        try
        {
            var result = await _service.UpdateOrder(orderId, orderUpdateDto);
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

    [HttpPost("{orderId:int}/costs/approve")]
    public async Task<ActionResult<OrderResponseDto>> ApproveCosts(int orderId)
    {
        try { return Ok(await _service.ApproveCostsAndCompleteOrder(orderId)); }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("{orderId:int}/cancel")]
    public async Task<ActionResult<OrderResponseDto>> Cancel(int orderId)
    {
        try { return Ok(await _service.CancelOrder(orderId)); }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }
}
