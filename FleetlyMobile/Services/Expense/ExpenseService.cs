using Fleetly.Shared.Client;
using Fleetly.Shared.Dto.ExpenseDtos;

namespace FleetlyMobile.Services.Expense
{
    public class ExpenseService(ApiClient api) : IExpenseService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/Expense";

        public async Task<ApiResponse<ExpenseResponseDto?>> GetById(int id)
        {
            return await _api.GetAsync<ExpenseResponseDto?>($"{BaseUrl}/{id}");
        }
    }
}
