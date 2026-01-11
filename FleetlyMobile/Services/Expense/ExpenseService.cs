using Fleetly.Shared.Client;
using Fleetly.Shared.Dto.ExpenseDtos;

namespace FleetlyMobile.Services.Expense
{
    public class ExpenseService(ApiClient api) : IExpenseService
    {
        private readonly ApiClient _api = api;
        private const string _baseUrl = "api/Expense";

        public async Task<ApiResponse<ExpenseResponseDto?>> GetById(int id)
        {
            return await _api.GetAsync<ExpenseResponseDto?>($"{_baseUrl}/{id}");
        }
    }
}
