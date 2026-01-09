using Fleetly.Shared.Dto.ExpenseDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using FleetlyBackend.Services.FileService;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.ExpenseService
{
    public class ExpenseService(FleetlyContext context, IFileService fileService) : IExpenseService
    {
        private readonly FleetlyContext _context = context;
        private readonly IFileService _fileService = fileService;

        public async Task<ExpenseResponseDto> Create(int orderId, ExpenseCreateDto dto)
        {
            if (!await _context.Orders.AnyAsync(o => o.Id == orderId))
                throw new ArgumentException("Podane zlecenie nie istnieje.");

            string? fileName = null;
            try
            {
                fileName = await _fileService.SaveFileAsync(dto.CostPhoto);
                var exp = new Expense
                {
                    OrderId = orderId,
                    Cost = dto.Cost,
                    IsFuelExpense = dto.IsFuelExpense,
                    CostPhotoUrl = fileName
                };

                _context.Expenses.Add(exp);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    try { if (fileName is not null) await _fileService.DeleteFileAsync(fileName); }
                    catch (InvalidOperationException ex) { throw new InvalidOperationException("Błąd podczas usuwania zdjęcia kosztu: " + ex.Message); }
                    throw;
                }

                return exp.ToResponseDto();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Błąd podczas zapisywania zdjęcia kosztu: " + ex.Message);
            }
        }

        public async Task<ExpenseResponseDto> Update(int id, ExpenseUpdateDto dto)
        {
            var exp = await _context.Expenses.FindAsync(id)
                ?? throw new ArgumentException("Nie znaleziono kosztu.");

            string? newFile = null;
            var oldFile = exp.CostPhotoUrl;

            if (dto.Cost.HasValue)
                exp.Cost = dto.Cost.Value;

            if (dto.IsFuelExpense.HasValue)
                exp.IsFuelExpense = dto.IsFuelExpense.Value;

            if (dto.CostPhoto is not null)
            {
                try
                {
                    newFile = await _fileService.SaveFileAsync(dto.CostPhoto);
                    exp.CostPhotoUrl = newFile;
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
                if (newFile is not null)
                {
                    try { await _fileService.DeleteFileAsync(newFile); }
                    catch (InvalidOperationException ex)
                    {
                        exp.CostPhotoUrl = oldFile;
                        throw new InvalidOperationException("Błąd podczas aktualizacji zdjęcia kosztu: " + ex.Message);
                    }
                }
                throw;
            }
            if (newFile is not null && !string.IsNullOrEmpty(oldFile) && oldFile != newFile)
            {
                try { await _fileService.DeleteFileAsync(oldFile); }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException("Błąd podczas usuwania starego zdjęcia kosztu: " + ex.Message);
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
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException("Błąd podczas usuwania zdjęcia kosztu." + ex.Message);
                }
            }

            return true;
        }
    }
}
