using FleetlyBackend.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FleetlyBackend.Helpers;
using Fleetly.Shared.Dto.ExpenseDtos;
using Fleetly.Shared.Dto.OrderDtos;

namespace FleetlyBackend.Controllers.OrderControllers;

[Route("api/worker/orders")]
[ApiController]
[Authorize(Roles = "Worker")]
public class WorkerOrderController(IOrderService service) : ControllerBase
{
    private readonly IOrderService _service = service;

    [HttpGet("my")]
    public async Task<ActionResult<List<OrderResponseDto>>> GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (!User.TryGetUserId(out var userId, out var error))
            return Unauthorized(error);

        return Ok(await _service.GetAllOrdersForWorker(userId, page, pageSize));
    }

    [HttpGet("/available")]
    public async Task<ActionResult<List<OrderResponseDto>>> GetAllAvailableOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (!User.TryGetUserId(out var userId, out var error))
            return Unauthorized(error);
        return Ok(await _service.GetAvailableOrdersForWorker(page, pageSize));
    }

    [HttpGet("{orderId:int}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrderById(int orderId)
    {
        if (!User.TryGetUserId(out var userId, out var error))
            return Unauthorized(error);
        try
        {
            var order = await _service.GetOrderForWorkerById(orderId, userId);
            if (order == null)
                return NotFound(new { error = "Zlecenie nie istnieje lub nie masz do niego dostępu." });
            return Ok(order);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("{orderId:int}/accept")]
    public async Task<ActionResult<OrderResponseDto>> Accept(int orderId)
    {
        if (!User.TryGetUserId(out var userId, out var error))
            return Unauthorized(error);

        try { return Ok(await _service.AcceptOrder(orderId, userId)); }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("{orderId:int}/resign")]
    public async Task<ActionResult<OrderResponseDto>> Resign(int orderId)
    {
        if (!User.TryGetUserId(out var userId, out var error))
            return Unauthorized(error);

        try { return Ok(await _service.ResignOrder(orderId, userId)); }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
    }

    [HttpPost("{orderId:int}/start")]
    public async Task<ActionResult<OrderResponseDto>> Start(int orderId)
    {
        if (!User.TryGetUserId(out var userId, out var error))
            return Unauthorized(error);
        try { return Ok(await _service.StartOrder(orderId, userId)); }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
    }

    [HttpPost("{orderId:int}/service/arrive")]
    public async Task<ActionResult<OrderResponseDto>> ArriveToService(int orderId)
    {
        if (!User.TryGetUserId(out var userId, out var error))
            return Unauthorized(error);

        try { return Ok(await _service.ArrivedToService(orderId, userId)); }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
    }

    [HttpPost("{orderId:int}/service/leave")]
    public async Task<ActionResult<OrderResponseDto>> LeaveService(int orderId)
    {
        if (!User.TryGetUserId(out var userId, out var error))
            return Unauthorized(error);

        try { return Ok(await _service.LeaveServiceLocation(orderId, userId)); }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
    }

    [HttpPost("{orderId:int}/arrive-client")]
    public async Task<ActionResult<OrderResponseDto>> ArriveClient(int orderId)
    {
        if (!User.TryGetUserId(out var userId, out var error))
            return Unauthorized(error);

        try { return Ok(await _service.ArrivedToClient(orderId, userId)); }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
    }

    [HttpPost("{orderId:int}/finish")]
    public async Task<ActionResult<OrderResponseDto>> Finish(int orderId)
    {
        if (!User.TryGetUserId(out var userId, out var error))
            return Unauthorized(error);

        try { return Ok(await _service.FinishOrderByWorker(orderId, userId)); }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
    }

    [HttpPost("{orderId:int}/costs")]
    public async Task<ActionResult<OrderResponseDto>> AddCost(int orderId, [FromForm] ExpenseCreateDto dto)
    {
        if (!User.TryGetUserId(out var userId, out var error))
            return Unauthorized(error);

        try { return Ok(await _service.AddCost(orderId, userId, dto)); }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
    }

    [HttpPut("{orderId:int}/costs/{expenseId:int}")]
    public async Task<ActionResult<OrderResponseDto>> UpdateCost(int orderId, int expenseId, [FromForm] ExpenseUpdateDto dto)
    {
        if (!User.TryGetUserId(out var userId, out var error))
            return Unauthorized(error);

        try { return Ok(await _service.UpdateCost(orderId, userId, expenseId, dto)); }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
    }

    [HttpDelete("{orderId:int}/costs/{expenseId:int}")]
    public async Task<ActionResult<OrderResponseDto>> DeleteCost(int orderId, int expenseId)
    {
        if (!User.TryGetUserId(out var userId, out var error))
            return Unauthorized(error);

        try { return Ok(await _service.DeleteCost(orderId, userId, expenseId)); }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
    }

    [HttpPost("{orderId:int}/costs/submit")]
    public async Task<ActionResult<OrderResponseDto>> SubmitCosts(int orderId)
    {
        if (!User.TryGetUserId(out var userId, out var error))
            return Unauthorized(error);

        try { return Ok(await _service.SubmitAllOrderCosts(orderId, userId)); }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
    }
}
