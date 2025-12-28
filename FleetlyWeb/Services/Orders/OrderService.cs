using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.OrderDtos;

namespace FleetlyWeb.Services.Orders
{
    public class OrderService(ApiClient api) : IOrderService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/Orders"; 

        public async Task<ApiResponse<PagedResult<OrderResponseDto>?>> GetAllOrdersAsync()
        {
            return await _api.GetAsync<PagedResult<OrderResponseDto>>(BaseUrl);
        }

        public async Task<ApiResponse<OrderResponseDto?>> GetOrderByIdAsync(int orderId)
        {
            return await _api.GetAsync<OrderResponseDto>($"{BaseUrl}/{orderId}");
        }

        public async Task<ApiResponse<OrderResponseDto?>> CreateOrderAsync(OrderCreateDto dto)
        {
            return await _api.PostAsync<OrderCreateDto, OrderResponseDto>(BaseUrl, dto);
        }

        public async Task<ApiResponse<OrderResponseDto?>> UpdateOrderAsync(int orderId, OrderUpdateDto dto)
        {
            return await _api.PutAsync<OrderUpdateDto, OrderResponseDto>($"{BaseUrl}/{orderId}", dto);
        }

        public async Task<ApiResponse<OrderResponseDto?>> CancelOrderAsync(int orderId)
        {
            return await _api.DeleteAsync<OrderResponseDto>($"{BaseUrl}/{orderId}");
        }


        public async Task<ApiResponse<OrderResponseDto?>> ApproveOrderAsync(int orderId)
        {
            return await _api.PostAsync<object, OrderResponseDto>($"{BaseUrl}/{orderId}/activate", null!);
        }

        public async Task<ApiResponse<OrderResponseDto?>> ApproveCostsAsync(int orderId)
        {
            return await _api.PostAsync<object, OrderResponseDto>($"{BaseUrl}/{orderId}/costs/approve", null!);
        }
    }
}