using Fleetly.Shared.Enums;
using Fleetly.Shared.Client;
using Fleetly.Shared.Dto.ProtocolDtos;

namespace FleetlyWeb.Services.Protocols
{
    public interface IProtocolService
    {
        Task<ApiResponse<ProtocolResponseDto?>> GetProtocolByOrderIdAsync(int orderId, ProtocolType? type = null);
    }
}
