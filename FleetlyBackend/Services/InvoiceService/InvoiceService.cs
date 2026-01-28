using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.InvoiceDtos;
using Fleetly.Shared.Enums;
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
        public async Task<PagedResult<InvoiceResponseDto>> GetAll(
            int page = 1,
            int pageSize = 10,
            bool showPaid = false,
            string? search = null)
        {
            var (skip, take) = PaginationHelper.Calculate(page, pageSize);
            var user = _http.CurrentUser();

            var query = _context.Invoices
                .AsNoTracking();

            if (user.IsClient())
            {
                var clientId = user.GetUserId();
                query = query.Where(i => i.Order.ClientId == clientId);
            }

            if (!showPaid)
            {
                query = query.Where(i => !i.IsPaid);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var phrase = search.Trim().ToLower();
                query = query.Where(i =>
                    i.Id.ToString().Contains(phrase) ||
                    i.OrderId.ToString().Contains(phrase) ||
                    (i.Order.Vehicle != null && i.Order.Vehicle.RegistrationNumber.ToLower().Contains(phrase))
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Include(i => i.Order)
                .ThenInclude(o => o.Vehicle)
                .ThenInclude(v => v.BrandModel)
                .ThenInclude(bm => bm.CarBrand)
                .OrderByDescending(i => i.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(i => i.ToResponseDto())
                .ToListAsync();

            return new PagedResult<InvoiceResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = take,
                PageNumber = page
            };
        }

        public async Task<InvoiceResponseDto?> GetById(int id)
        {
            var user = _http.CurrentUser();
            var inv = await _context.Invoices
                .Include(i => i.Order)
                .ThenInclude(o => o.Vehicle)
                .ThenInclude(v => v.BrandModel)
                .ThenInclude(bm => bm.CarBrand)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inv is null) return null;

            if (user.IsClient() && user.GetUserId() != inv.Order.ClientId)
                throw new UnauthorizedAccessException("Nie masz uprawnień do przeglądania tej faktury.");

            return inv.ToResponseDto();
        }

        public async Task Create(InvoiceCreateDto dto)
        {
            var orderExists = await _context.Orders.AnyAsync(o => o.Id == dto.OrderId);
            if (!orderExists)
                throw new ArgumentException("Podane zlecenie nie istnieje.");

            var exists = await _context.Invoices.AnyAsync(i => i.OrderId == dto.OrderId);
            if (exists)
                throw new InvalidOperationException("Faktura już istnieje.");

            var now = DateTime.UtcNow;
            var thirtyDays = now.AddDays(30);

            var invoice = new Invoice
            {
                OrderId = dto.OrderId,
                Sum = dto.Sum,
                IsPaid = false,
                DueDate = thirtyDays,
                CreatedAt = now,
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task Update(int id, InvoiceUpdateDto dto)
        {
            var invoice = await _context.Invoices.FindAsync(id)
                ?? throw new ArgumentException("Faktura nie istnieje.");

            if (dto.Sum.HasValue && dto.Sum != invoice.Sum)
                invoice.Sum = dto.Sum.Value;

            invoice.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task ConfirmPayment(int invoiceId, string methodString)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId)
                          ?? throw new ArgumentException("Faktura nie istnieje.");

            if (invoice.IsPaid) return;

            invoice.IsPaid = true;
            invoice.DateOfPayment = DateTime.UtcNow;

            invoice.MethodOfPayment = (Enum.TryParse<MethodOfPayment>(methodString, true, out var methodEnum))
                ? methodEnum : invoice.MethodOfPayment = MethodOfPayment.Card;

            invoice.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
