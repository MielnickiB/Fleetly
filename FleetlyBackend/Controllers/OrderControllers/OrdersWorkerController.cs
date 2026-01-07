using FleetlyBackend.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FleetlyBackend.Helpers;
using Fleetly.Shared.Dto.ExpenseDtos;
using Fleetly.Shared.Dto.OrderDtos;
using Fleetly.Shared.Dto;

namespace FleetlyBackend.Controllers.OrderControllers;

[Route("api/worker/orders")]
[ApiController]
[Authorize(Roles = "Worker")]
public class WorkerOrderController(IOrderService service) : ControllerBase
{
    private readonly IOrderService _service = service;

    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderResponseDto>>> GetMyOrders()
    {
        return Ok(await _service.GetAllOrders());
    }

    [HttpGet("available")]
    public async Task<ActionResult<PagedResult<OrderLiteDto>>> GetAllAvailableOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool descending = false)
    {
        return Ok(await _service.GetAvailableOrders(page, pageSize, search, sortBy, descending));
    }

    [HttpGet("{orderId:int}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrderById(int orderId)
    {
        try
        {
            var order = await _service.GetOrderById(orderId);
            return order is null ? NotFound("Zlecenie nie istnieje.") : Ok(order);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost("{orderId:int}/accept")]
    public async Task<ActionResult> Accept(int orderId)
    {
        try 
        { 
            await _service.AcceptOrder(orderId); 
            return NoContent();
        }
        catch (ArgumentException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpPost("{orderId:int}/resign")]
    public async Task<ActionResult<OrderResponseDto>> Resign(int orderId)
    {
        try { return Ok(await _service.ResignOrder(orderId)); }
        catch (ArgumentException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpPost("{orderId:int}/start")]
    public async Task<ActionResult<OrderResponseDto>> Start(int orderId)
    {
        try { return Ok(await _service.StartOrder(orderId)); }
        catch (ArgumentException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpPost("{orderId:int}/arrive")]
    public async Task<ActionResult<OrderResponseDto>> ArriveClient(int orderId)
    {
        try { return Ok(await _service.ArrivedToClient(orderId)); }
        catch (ArgumentException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpPost("{orderId:int}/finish")]
    public async Task<ActionResult<OrderResponseDto>> Finish(int orderId)
    {
        try { return Ok(await _service.FinishOrderByWorker(orderId)); }
        catch (ArgumentException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpPost("{orderId:int}/costs")]
    public async Task<ActionResult<OrderResponseDto>> AddCost(int orderId, [FromForm] ExpenseCreateDto dto)
    {
        try { return Ok(await _service.AddCost(orderId, dto)); }
        catch (ArgumentException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpPut("{orderId:int}/costs/{expenseId:int}")]
    public async Task<ActionResult<OrderResponseDto>> UpdateCost(int orderId, int expenseId, [FromForm] ExpenseUpdateDto dto)
    {
        try { return Ok(await _service.UpdateCost(orderId, expenseId, dto)); }
        catch (ArgumentException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpDelete("{orderId:int}/costs/{expenseId:int}")]
    public async Task<ActionResult<OrderResponseDto>> DeleteCost(int orderId, int expenseId)
    {
        try { return Ok(await _service.DeleteCost(orderId, expenseId)); }
        catch (ArgumentException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpPost("{orderId:int}/costs/submit")]
    public async Task<ActionResult<OrderResponseDto>> SubmitCosts(int orderId)
    {

        try { return Ok(await _service.SubmitAllOrderCosts(orderId)); }
        catch (ArgumentException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }
}
