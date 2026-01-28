using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.InvoiceDtos;
using Fleetly.Shared.Client;

namespace FleetlyWeb.Services.Invoices
{
    public interface IInvoiceService
    {
        Task<ApiResponse<PagedResult<InvoiceResponseDto>?>> GetAllAsync(int page = 1, int pageSize = 10, bool showPaid = false, string? search = null);
        Task<ApiResponse<InvoiceResponseDto?>> GetByIdAsync(int id);
        Task<ApiResponse<bool>> CreateAsync(InvoiceCreateDto dto);
        Task<ApiResponse<bool>> UpdateAsync(int id, InvoiceUpdateDto dto);
        Task<ApiResponse<PaymentInitResponseDto?>> InitOnlinePaymentAsync(int id);
        Task<ApiResponse<bool>> ConfirmOnlinePaymentAsync(string sessionId);
    }
}