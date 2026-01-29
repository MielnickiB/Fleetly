using Fleetly.Shared.Enums;
using Fleetly.Shared.Client;
using Fleetly.Shared.Dto.ProtocolDtos;

namespace FleetlyWeb.Services.Protocols
{
    public class ProtocolService(ApiClient api) : IProtocolService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/Protocol";

        public async Task<ApiResponse<ProtocolResponseDto?>> GetProtocolByOrderIdAsync(int orderId, ProtocolType? type = null)
        {
            var url = $"{BaseUrl}/order/{orderId}";
            if (type.HasValue)
            {
                url += $"?type={(int)type.Value}";
            }
            return await _api.GetAsync<ProtocolResponseDto>(url);
        }
    }
}
