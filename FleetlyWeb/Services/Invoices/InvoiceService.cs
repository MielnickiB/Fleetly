using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.InvoiceDtos;
using Fleetly.Shared.Client;

namespace FleetlyWeb.Services.Invoices
{
    public class InvoiceService(ApiClient api) : IInvoiceService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/Invoice";

        public async Task<ApiResponse<PagedResult<InvoiceResponseDto>?>> GetAllAsync(
            int page = 1,
            int pageSize = 10,
            bool showPaid = false,
            string? search = null)
        {
            var url = $"{BaseUrl}?page={page}&pageSize={pageSize}&showPaid={showPaid}";

            if (!string.IsNullOrWhiteSpace(search))
            {
                url += $"&search={Uri.EscapeDataString(search)}";
            }

            return await _api.GetAsync<PagedResult<InvoiceResponseDto>>(url);
        }

        public async Task<ApiResponse<InvoiceResponseDto?>> GetByIdAsync(int id)
        {
            return await _api.GetAsync<InvoiceResponseDto>($"{BaseUrl}/{id}");
        }

        public async Task<ApiResponse<bool>> CreateAsync(InvoiceCreateDto dto)
        {
            return await _api.PostAsync<InvoiceCreateDto, bool>(BaseUrl, dto);
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int id, InvoiceUpdateDto dto)
        {
            return await _api.PutAsync<InvoiceUpdateDto, bool>($"{BaseUrl}/{id}", dto);
        }

        public async Task<ApiResponse<PaymentInitResponseDto?>> InitOnlinePaymentAsync(int id)
        {
            return await _api.PostAsync<object, PaymentInitResponseDto>($"{BaseUrl}/{id}/pay-online", null!);
        }

        public async Task<ApiResponse<bool>> ConfirmOnlinePaymentAsync(string sessionId)
        {
            var url = $"{BaseUrl}/confirm-payment?sessionId={Uri.EscapeDataString(sessionId)}";

            return await _api.PostAsync<object, bool>(url, null!);
        }
    }
}