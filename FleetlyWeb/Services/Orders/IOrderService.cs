using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.OrderDtos;
using Fleetly.Shared.Client;

namespace FleetlyWeb.Services.Orders
{
    public interface IOrderService
    {
        Task<ApiResponse<PagedResult<OrderResponseDto>?>> GetAllOrdersAsync(bool includeInactive);
        Task<ApiResponse<OrderResponseDto?>> GetOrderByIdAsync(int orderId);

        Task<ApiResponse<OrderResponseDto?>> CreateOrderAsync(OrderCreateDto dto);
        Task<ApiResponse<OrderResponseDto?>> UpdateOrderAsync(int orderId, OrderUpdateDto dto);

        Task<ApiResponse<OrderResponseDto?>> CancelOrderAsync(int orderId);
        Task<ApiResponse<OrderResponseDto?>> ApproveOrderAsync(int orderId);
        Task<ApiResponse<bool>> ApproveCostsAsync(int orderId);
        Task<ApiResponse<int>> CalculateDistanceAsync(string start, string end);
    }
}