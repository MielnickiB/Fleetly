using Fleetly.Shared.Dto.ExpenseDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Extensions;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using FleetlyBackend.Services.FileService;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.ExpenseService
{
    public class ExpenseService(FleetlyContext context, IFileService fileService, IHttpContextAccessor http) : IExpenseService
    {
        private readonly FleetlyContext _context = context;
        private readonly IFileService _fileService = fileService;
        private readonly IHttpContextAccessor _http = http;

        public async Task<ExpenseResponseDto?> GetById(int id)
        {
            var user = _http.CurrentUser();
            var userId = user.GetUserId();

            var query = _context.Expenses
                .AsNoTracking()
                .Include(e => e.Order)
                .AsQueryable();

            if (user.IsAdmin())
            {
            }
            else if (user.IsWorker())
            {
                query = query.Where(e => e.Order.WorkerId == userId);
            }
            else if (user.IsClient())
            {
                query = query.Where(e => e.Order.ClientId == userId);
            }
            else
            {
                throw new UnauthorizedAccessException("Brak uprawnień.");
            }

            var expense = await query.FirstOrDefaultAsync(e => e.Id == id);

            return expense?.ToResponseDto();
        }

        public async Task<ExpenseResponseDto> Create(int orderId, ExpenseCreateDto dto)
        {
            if (!await _context.Orders.AnyAsync(o => o.Id == orderId))
                throw new ArgumentException("Podane zlecenie nie istnieje.");

            string? savedFilePath = null;

            string folderStructure = Path.Combine("Orders", orderId.ToString(), "Costs");

            try
            {
                savedFilePath = await _fileService.SaveFileAsync(dto.CostPhoto, folderStructure);

                var exp = new Expense
                {
                    OrderId = orderId,
                    Cost = dto.Cost,
                    IsFuelExpense = dto.IsFuelExpense,
                    CostPhotoUrl = savedFilePath
                };

                _context.Expenses.Add(exp);

                await _context.SaveChangesAsync();
                return exp.ToResponseDto();
            }
            catch
            {
                if (savedFilePath is not null)
                {
                    await _fileService.DeleteFileAsync(savedFilePath);
                }
                throw;
            }
        }

        public async Task<ExpenseResponseDto> Update(int id, ExpenseUpdateDto dto)
        {
            var exp = await _context.Expenses.FindAsync(id)
                ?? throw new ArgumentException("Nie znaleziono kosztu.");

            string? newFilePath = null;
            var oldFilePath = exp.CostPhotoUrl;
            bool fileChanged = false;

            if (dto.Cost.HasValue)
                exp.Cost = dto.Cost.Value;

            if (dto.IsFuelExpense.HasValue)
                exp.IsFuelExpense = dto.IsFuelExpense.Value;

            if (dto.CostPhoto is not null)
            {
                string folderStructure = Path.Combine("Orders", exp.OrderId.ToString(), "Costs");
                try
                {
                    newFilePath = await _fileService.SaveFileAsync(dto.CostPhoto, folderStructure);
                    exp.CostPhotoUrl = newFilePath;
                    fileChanged = true;
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException("Błąd podczas aktualizacji zdjęcia kosztu: " + ex.Message);
                }
            }

            exp.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                if (newFilePath is not null)
                {
                    await _fileService.DeleteFileAsync(newFilePath);
                }
                throw;
            }

            if (fileChanged && !string.IsNullOrEmpty(oldFilePath) && oldFilePath != newFilePath)
            {
                try
                {
                    await _fileService.DeleteFileAsync(oldFilePath);
                }
                catch
                {
                    // Orphaned file, Ignoruje ponieważ koszt został zaktualizowany w bazie danych i to jest najważniejsze.
                }
            }

            return exp.ToResponseDto();
        }

        public async Task<bool> Delete(int id)
        {
            var exp = await _context.Expenses.FindAsync(id) ??
                throw new ArgumentException("Nie znaleziono kosztu.");

            var fileToDelete = exp.CostPhotoUrl;

            _context.Expenses.Remove(exp);

            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(fileToDelete))
            {
                try { await _fileService.DeleteFileAsync(fileToDelete); }
                catch
                {
                    // Orphaned file, Ignoruje ponieważ koszt został usunięty z bazy danych i to jest najważniejsze.
                }
            }
            return true;
        }
    }
}
