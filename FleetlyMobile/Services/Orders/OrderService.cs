using Fleetly.Shared.Client;
using Fleetly.Shared.Dto.OrderDtos;
using Fleetly.Shared.Dto;

namespace FleetlyMobile.Services.Orders
{
    public class OrderService(ApiClient api) : IOrderService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/worker/orders";

        public async Task<ApiResponse<PagedResult<OrderLiteDto>?>> GetAllAvailableAsync(
            int page = 1,
            int pageSize = 10,
            string? search = null,
            string? sortBy = null,
            bool descending = false)
        {
            var url = $"{BaseUrl}/available?page={page}&pageSize={pageSize}";

            if (!string.IsNullOrEmpty(search))
                url += $"&search={Uri.EscapeDataString(search)}";

            if (!string.IsNullOrEmpty(sortBy))
            {
                url += $"&sortBy={sortBy}";
                url += $"&descending={descending}";
            }
            return await _api.GetAsync<PagedResult<OrderLiteDto>?>(url);
        }

        public async Task<ApiResponse<OrderResponseDto?>> GetOrderByIdAsync(int orderId)
        {
            return await _api.GetAsync<OrderResponseDto?>($"{BaseUrl}/{orderId}");
        }

        public async Task<ApiResponse<bool>> AcceptOrderAsync(int orderId)
        {
            return await _api.PostNoResultAsync<object>($"{BaseUrl}/{orderId}/accept", null!);
        }

        public async Task<ApiResponse<bool>> ResignOrderAsync(int orderId)
        {
            return await _api.PostNoResultAsync<object>($"{BaseUrl}/{orderId}/resign", null!);
        }
    }
}
