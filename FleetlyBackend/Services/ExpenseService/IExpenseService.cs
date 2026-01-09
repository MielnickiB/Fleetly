using Fleetly.Shared.Dto.ExpenseDtos;

namespace FleetlyBackend.Services.ExpenseService
{
    public interface IExpenseService
    {
        Task Create(int orderId, ExpenseCreateDto dto);
        Task Update(int id, ExpenseUpdateDto dto);
        Task Delete(int id);
    }
}
