using Fleetly.Shared.Dto.ExpenseDtos;
using FleetlyBackend.Services.ExpenseService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseController(IExpenseService service) : ControllerBase
    {
        private readonly IExpenseService _service = service;

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ExpenseResponseDto>> GetExpenseById(int id)
        {
            try
            {
                var expense = await _service.GetById(id);
                return expense is null ? NotFound("Nie znaleziono danego wydatku.") : Ok(expense);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
