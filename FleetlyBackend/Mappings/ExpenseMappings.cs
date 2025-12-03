using Fleetly.Shared.Dto.ExpenseDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Mappings
{
    public static class ExpenseMappings
    {
        public static ExpenseResponseDto ToResponseDto(this Expense e)
            => new()
            {
                Id = e.Id,
                OrderId = e.OrderId,
                Cost = e.Cost,
                CostPhotoUrl = e.CostPhotoUrl,
                IsFuelExpense = e.IsFuelExpense
            };
    }
}
