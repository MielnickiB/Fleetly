using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.ExpenseDtos;
using Fleetly.Shared.Dto.OrderDtos;

namespace FleetlyBackend.Services.OrderService
{
    public interface IOrderService
    {
        public Task<OrderResponseDto?> GetOrderById(int orderId);
        public Task<PagedResult<OrderResponseDto>> GetAllOrders();
        public Task<PagedResult<OrderResponseDto>> GetAvailableOrders();
        public Task<OrderResponseDto> CreateOrder(OrderCreateDto orderCreateDto);
        public Task<OrderResponseDto> UpdateOrder(int orderId, OrderUpdateDto orderUpdateDto, int? userId = null);
        public Task<OrderResponseDto> CancelOrder(int orderId, int? userId = null);
        public Task<OrderResponseDto> AcceptOrder(int orderId, int workerId);
        public Task<OrderResponseDto> ResignOrder(int orderId, int workerId);
        public Task<OrderResponseDto> StartOrder(int orderId, int workerId);
        public Task<OrderResponseDto> ArrivedToService(int orderId, int workerId);
        public Task<OrderResponseDto> LeaveServiceLocation(int orderId, int workerId);
        public Task<OrderResponseDto> ArrivedToClient(int orderId, int workerId);
        public Task<OrderResponseDto> FinishOrderByWorker(int orderId, int workerId);
        public Task<OrderResponseDto> SubmitAllOrderCosts(int orderId, int workerId);
        public Task<OrderResponseDto> ApproveCostsAndCompleteOrder(int orderId);
        public Task<OrderResponseDto> ApproveOrder(int orderId);
        public Task<OrderResponseDto> AddCost(int orderId, int workerId, ExpenseCreateDto dto);
        public Task<OrderResponseDto> UpdateCost(int orderId, int workerId, int expenseId, ExpenseUpdateDto dto);
        public Task<OrderResponseDto> DeleteCost(int orderId, int workerId, int expenseId);
    }
}
