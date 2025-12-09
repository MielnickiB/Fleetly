using Fleetly.Shared.Dto.InvoiceDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Extensions;
using FleetlyBackend.Helpers;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.InvoiceService
{
    public class InvoiceService(FleetlyContext context, IHttpContextAccessor http) : IInvoiceService
    {
        private readonly FleetlyContext _context = context;
        private readonly IHttpContextAccessor _http = http;
        public async Task<List<InvoiceResponseDto>> GetAll(int page = 1, int pageSize = 10)
        {
            var (skip, take) = PaginationHelper.Calculate(page, pageSize);
            var user = _http.CurrentUser();

            var query = _context.Invoices.AsNoTracking().AsQueryable();

            if (user.IsClient())
            {
                var clientId = user.GetUserId();
                query = query.Where(i => i.Order.ClientId == clientId);
            }

            return await query
                .OrderBy(i => i.Id)
                .Skip(skip)
                .Take(take)
                .Select(i => i.ToResponseDto())
                .ToListAsync();
        }

        public async Task<InvoiceResponseDto?> GetById(int id)
        {
            var user = _http.CurrentUser();
            var inv = await _context.Invoices.FindAsync(id);
            if (inv is null)
                return null;
            if (user.IsClient() && user.GetUserId() != inv.Order.ClientId)
                throw new UnauthorizedAccessException("Nie masz uprawnień do przeglądania tej faktury.");
            return inv?.ToResponseDto();
        }

        public async Task<InvoiceResponseDto> Create(InvoiceCreateDto dto)
        {
            var orderExists = await _context.Orders.AnyAsync(o => o.Id == dto.OrderId);
            if (!orderExists)
                throw new ArgumentException("Podane zamówienie nie istnieje.");

            var invoice = new Invoice
            {
                OrderId = dto.OrderId,
                Sum = dto.Sum,
                IsPaid = false,
                MethodOfPayment = dto.MethodOfPayment
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return invoice.ToResponseDto();
        }

        public async Task<InvoiceResponseDto> Update(int id, InvoiceUpdateDto dto)
        {
            var invoice = await _context.Invoices.FindAsync(id)
                ?? throw new ArgumentException("Faktura nie istnieje.");

            if (dto.Sum.HasValue)
                invoice.Sum = dto.Sum.Value;

            if (dto.IsPaid.HasValue)
            {
                invoice.IsPaid = dto.IsPaid.Value;

                invoice.DateOfPayment = dto.IsPaid.Value == true ? DateTime.UtcNow : null;
            }

            if (dto.MethodOfPayment.HasValue)
                invoice.MethodOfPayment = dto.MethodOfPayment;

            invoice.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return invoice.ToResponseDto();
        }

        public async Task<bool> Delete(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id) ??
                throw new ArgumentException("Faktura nie istnieje.");

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
