using Fleetly.Shared.Dto.InvoiceDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Helpers;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.InvoiceService
{
    public class InvoiceService(FleetlyContext context) : IInvoiceService
    {
        private readonly FleetlyContext _context = context;
        public async Task<List<InvoiceResponseDto>> GetAll(int page, int pageSize)
        {
            var (skip, take) = PaginationHelper.Calculate(page, pageSize);

            return await _context.Invoices
                .AsNoTracking()
                .OrderBy(i => i.Id)
                .Skip(skip)
                .Take(take)
                .Select(i => i.ToResponseDto())
                .ToListAsync();
        }

        public async Task<List<InvoiceResponseDto>> GetForClient(int clientId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(c => c.Id == clientId) 
                ?? throw new ArgumentException("Podany klient nie istnieje.");

            if (user.Role == null || user.Role.RoleName != "Client")
                throw new InvalidOperationException("Podany użytkownik nie jest klientem.");

            return await _context.Invoices
                .AsNoTracking()
                .Where(i => i.Order.ClientId == clientId)
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => i.ToResponseDto())
                .ToListAsync();
        }

        public async Task<InvoiceResponseDto?> GetById(int id)
        {
            var inv = await _context.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
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

                invoice.DateOfPayment = dto.IsPaid.Value ? DateTime.UtcNow : null;
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
