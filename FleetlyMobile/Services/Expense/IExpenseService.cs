using Fleetly.Shared.Client;
using Fleetly.Shared.Dto.ExpenseDtos;

namespace FleetlyMobile.Services.Expense
{
    public interface IExpenseService
    {
        public Task<ApiResponse<ExpenseResponseDto?>> GetById(int id);
    }
}
