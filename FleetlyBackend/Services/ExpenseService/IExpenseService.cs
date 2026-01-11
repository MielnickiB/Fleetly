using Fleetly.Shared.Dto.ExpenseDtos;

namespace FleetlyBackend.Services.ExpenseService
{
    public interface IExpenseService
    {
        Task<ExpenseResponseDto?> GetById(int id);
        Task<ExpenseResponseDto> Create(int orderId, ExpenseCreateDto dto);
        Task<ExpenseResponseDto> Update(int id, ExpenseUpdateDto dto);
        Task<bool> Delete(int id);
    }
}
