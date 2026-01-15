using Fleetly.Shared.Client;
using Fleetly.Shared.Dto.DamageDtos;
using Fleetly.Shared.Dto.ProtocolDtos;
using Fleetly.Shared.Enums;

namespace FleetlyMobile.Services.Protocol
{
    public interface IProtocolService
    {
        Task<ApiResponse<ProtocolResponseDto?>> GetProtocolByOrderIdAsync(int orderId, ProtocolType? type = null);

        Task<ApiResponse<ProtocolResponseDto?>> StartProtocolAsync(ProtocolInitDto dto);

        Task<ApiResponse<ProtocolResponseDto?>> AddPhotoAsync(ProtocolPhotoDto dto, string filePath);
        Task<ApiResponse<ProtocolResponseDto?>> AddDamageAsync(DamageCreateDto dto, string filePath);

        Task<ApiResponse<ProtocolResponseDto?>> DeleteDamageAsync(int damageId);
        Task<ApiResponse<ProtocolResponseDto?>> MarkDamageFixedAsync(int protocolId, int damageId);

        Task<ApiResponse<ProtocolResponseDto?>> FinishProtocolAsync(ProtocolFinishDto dto, string signaturePath);

        Task<ApiResponse<bool>> UpdateStepAsync(int protocolId, int step);
    }
}