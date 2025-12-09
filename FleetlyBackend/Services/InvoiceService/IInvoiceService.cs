using Fleetly.Shared.Dto.InvoiceDtos;

namespace FleetlyBackend.Services.InvoiceService
{
    public interface IInvoiceService
    {
        Task<List<InvoiceResponseDto>> GetAll(int page = 1, int pageSize = 10);
        Task<InvoiceResponseDto?> GetById(int id);
        Task<InvoiceResponseDto> Create(InvoiceCreateDto dto);
        Task<InvoiceResponseDto> Update(int id, InvoiceUpdateDto dto);
        Task<bool> Delete(int id);
    }
}
