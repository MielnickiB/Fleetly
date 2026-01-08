using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.ExpenseDtos;
using Fleetly.Shared.Dto.OrderDtos;

namespace FleetlyBackend.Services.OrderService
{
    public interface IOrderService
    {
        public Task<OrderResponseDto?> GetOrderById(int orderId);
        public Task<PagedResult<OrderResponseDto>> GetAllOrders();
        public Task<PagedResult<OrderLiteDto>> GetAvailableOrders(
            int page = 1, 
            int pageSize = 10,
            string? search = null,
            string? sortBy = null,
            bool descending = false);
        public Task<OrderResponseDto> CreateOrder(OrderCreateDto orderCreateDto);
        public Task<OrderResponseDto> UpdateOrder(int orderId, OrderUpdateDto orderUpdateDto);
        public Task<OrderResponseDto> CancelOrder(int orderId);
        public Task AcceptOrder(int orderId);
        public Task ResignOrder(int orderId);
        public Task<OrderResponseDto> StartOrder(int orderId);
        public Task<OrderResponseDto> ArrivedToClient(int orderId);
        public Task<OrderResponseDto> FinishOrderByWorker(int orderId);
        public Task<OrderResponseDto> SubmitAllOrderCosts(int orderId);
        public Task<OrderResponseDto> ApproveCostsAndCompleteOrder(int orderId);
        public Task<OrderResponseDto> ApproveOrder(int orderId);
        public Task<OrderResponseDto> AddCost(int orderId, ExpenseCreateDto dto);
        public Task<OrderResponseDto> UpdateCost(int orderId, int expenseId, ExpenseUpdateDto dto);
        public Task<OrderResponseDto> DeleteCost(int orderId, int expenseId);
    }
}
