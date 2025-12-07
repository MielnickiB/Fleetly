using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.OrderDtos;

namespace FleetlyWeb.Services
{
    public interface IOrderService
    {
        Task<PagedResult<OrderResponseDto>> GetAllOrdersAsync();
    }
}
