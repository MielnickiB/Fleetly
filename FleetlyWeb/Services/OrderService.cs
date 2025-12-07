using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.OrderDtos;

namespace FleetlyWeb.Services
{
    public class OrderService(ApiClient api) : IOrderService
    {
        private readonly ApiClient _api = api;
        public async Task<PagedResult<OrderResponseDto>> GetAllOrdersAsync()
        {
            var resp = await _api.GetAsync<PagedResult<OrderResponseDto>>("api/admin/orders");
            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się pobrać zleceń.");
            }
            if (resp.Data is null)
            {
                throw new Exception("Nie udało się pobrać zleceń.");
            }
            return resp.Data!;
        }
    }
}
