using Fleetly.Shared.Dto.InvoiceDtos;

namespace FleetlyBackend.Services.InvoiceService
{
    public interface IInvoiceService
    {
        Task<List<InvoiceResponseDto>> GetAll(int page, int pageSize);
        Task<List<InvoiceResponseDto>> GetForClient(int clientId);
        Task<InvoiceResponseDto?> GetById(int id);
        Task<InvoiceResponseDto> Create(InvoiceCreateDto dto);
        Task<InvoiceResponseDto> Update(int id, InvoiceUpdateDto dto);
        Task<bool> Delete(int id);
    }
}
