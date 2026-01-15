using System.Globalization;
using System.Net.Http.Headers;
using Fleetly.Shared.Client;
using Fleetly.Shared.Dto.DamageDtos;
using Fleetly.Shared.Dto.ProtocolDtos;
using Fleetly.Shared.Enums;

namespace FleetlyMobile.Services.Protocol
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

        public async Task<ApiResponse<ProtocolResponseDto?>> StartProtocolAsync(ProtocolInitDto dto)
        {
            return await _api.PostAsync<ProtocolInitDto, ProtocolResponseDto>($"{BaseUrl}/start", dto);
        }

        public async Task<ApiResponse<ProtocolResponseDto?>> AddPhotoAsync(ProtocolPhotoDto dto, string filePath)
        {
            using var content = new MultipartFormDataContent
            {
                { new StringContent(dto.ProtocolId.ToString()), nameof(dto.ProtocolId) },
                { new StringContent(((int)dto.Side).ToString()), nameof(dto.Side) }
            };

            AddFileToContent(content, filePath, "Photo");

            return await _api.PostMultipartAsync<ProtocolResponseDto>($"{BaseUrl}/{dto.ProtocolId}/photos", content);
        }

        public async Task<ApiResponse<ProtocolResponseDto?>> AddDamageAsync(DamageCreateDto dto, string filePath)
        {
            using var content = new MultipartFormDataContent
            {
                { new StringContent(dto.ProtocolId.ToString()), nameof(dto.ProtocolId) },
                { new StringContent(((int)dto.DamageSide).ToString()), nameof(dto.DamageSide) },
                { new StringContent(((int)dto.DamagePart).ToString()), nameof(dto.DamagePart) },
                { new StringContent(((int)dto.DamageType).ToString()), nameof(dto.DamageType) }
            };

            if (!string.IsNullOrEmpty(dto.Description))
            {
                content.Add(new StringContent(dto.Description), nameof(dto.Description));
            }

            AddFileToContent(content, filePath, "Photo");

            return await _api.PostMultipartAsync<ProtocolResponseDto>($"{BaseUrl}/{dto.ProtocolId}/damages", content);
        }

        public async Task<ApiResponse<ProtocolResponseDto?>> DeleteDamageAsync(int damageId)
        {
            return await _api.DeleteAsync<ProtocolResponseDto>($"api/Protocol/damages/{damageId}");
        }

        public async Task<ApiResponse<ProtocolResponseDto?>> MarkDamageFixedAsync(int protocolId, int damageId)
        {
            return await _api.PatchAsync<ProtocolResponseDto>($"{BaseUrl}/{protocolId}/damages/{damageId}/fix", null);
        }

        public async Task<ApiResponse<ProtocolResponseDto?>> FinishProtocolAsync(ProtocolFinishDto dto, string signaturePath)
        {
            using var content = new MultipartFormDataContent
            {
                { new StringContent(dto.ProtocolId.ToString()), nameof(dto.ProtocolId) },
                { new StringContent(dto.Mileage.ToString(CultureInfo.InvariantCulture)), nameof(dto.Mileage) },
                { new StringContent(dto.FuelLevel.ToString(CultureInfo.InvariantCulture)), nameof(dto.FuelLevel) }
            };

            if (!string.IsNullOrEmpty(dto.Notes)) content.Add(new StringContent(dto.Notes), nameof(dto.Notes));

            content.Add(new StringContent(dto.HasRegistrationDocument.ToString()), nameof(dto.HasRegistrationDocument));
            content.Add(new StringContent(dto.HasServiceBook.ToString()), nameof(dto.HasServiceBook));
            content.Add(new StringContent(dto.HasInsurancePolicy.ToString()), nameof(dto.HasInsurancePolicy));
            content.Add(new StringContent(dto.NumberOfKeys.ToString()), nameof(dto.NumberOfKeys));

            AddFileToContent(content, signaturePath, "SignaturePhoto", "image/png");

            return await _api.PostMultipartAsync<ProtocolResponseDto>($"{BaseUrl}/{dto.ProtocolId}/finish", content);
        }

        public async Task<ApiResponse<bool>> UpdateStepAsync(int protocolId, int step)
        {
            return await _api.PatchAsync<bool>($"{BaseUrl}/{protocolId}/step/{step}", null);
        }

        private static void AddFileToContent(MultipartFormDataContent content, string filePath, string formKey, string contentType = "image/jpeg")
        {
            if (File.Exists(filePath))
            {
                using var fileStream = File.OpenRead(filePath);
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                content.Add(fileContent, formKey, Path.GetFileName(filePath));
            }
        }
    }
}