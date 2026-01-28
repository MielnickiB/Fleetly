using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.InvoiceDtos;

namespace FleetlyBackend.Services.InvoiceService
{
    public interface IInvoiceService
    {
        Task<PagedResult<InvoiceResponseDto>> GetAll(
            int page = 1, 
            int pageSize = 10, 
            bool showPaid = false, 
            string? search = null);
        Task<InvoiceResponseDto?> GetById(int id);
        Task Create(InvoiceCreateDto dto);
        Task Update(int id, InvoiceUpdateDto dto);
        Task ConfirmPayment(string sessionId, string methodString);
    }
}
