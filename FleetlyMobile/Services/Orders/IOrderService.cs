using Fleetly.Shared.Dto.OrderDtos;
using Fleetly.Shared.Client;
using Fleetly.Shared.Dto;

namespace FleetlyMobile.Services.Orders
{
    public interface IOrderService
    {
        public Task<ApiResponse<PagedResult<OrderLiteDto>?>> GetAllAvailableAsync(
            int page = 1, 
            int pageSize = 10,
            string? search = null,
            string? sortBy = null,
            bool descending = false);
        public Task<ApiResponse<OrderResponseDto?>> GetOrderByIdAsync(int orderId);
        public Task<ApiResponse<bool>> AcceptOrderAsync(int orderId);
        public Task<ApiResponse<bool>> ResignOrderAsync(int orderId);
    }
}
