using Fleetly.Shared.Dto.CostLimitDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Helpers;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.CostLimitService
{
    public class CostLimitService(FleetlyContext context) : ICostLimitService
    {
        private readonly FleetlyContext _context = context;

        public async Task<List<CostLimitResponseDto>> GetAll(int page = 1, int pageSize = 10)
        {
            var (skip, safe) = PaginationHelper.Calculate(page, pageSize);

            return await _context.CostLimits
                .AsNoTracking()
                .OrderBy(c => c.RangeOfKmMin)
                .Skip(skip)
                .Take(safe)
                .Select(c => c.ToResponseDto())
                .ToListAsync();
        }

        public async Task<CostLimitResponseDto?> Get(int id)
        {
            var c = await _context.CostLimits.FindAsync(id);
            return c?.ToResponseDto();
        }

        public async Task<CostLimitResponseDto> GetByRangeOfKm(int rangeOfKm)
        {
            var c = await _context.CostLimits
                .AsNoTracking()
                .FirstOrDefaultAsync(c => rangeOfKm >= c.RangeOfKmMin && rangeOfKm <= c.RangeOfKmMax);
            return c is null
                ? throw new ArgumentException("Nie znaleziono limitu kosztów dla podanego zakresu kilometrów.")
                : c.ToResponseDto();
        }

        public async Task<CostLimitResponseDto> Create(CostLimitCreateDto dto)
        {
            if (dto.RangeOfKmMin <= 0 || dto.RangeOfKmMax <= 0 || dto.BaseSalary <= 0 || dto.MaxSalary <= 0 || dto.MaxCosts <= 0)
                throw new InvalidOperationException("Wartości muszą być większe od 0.");

            if (dto.BaseSalary > dto.MaxSalary)
                throw new InvalidOperationException("Podstawowe wynagrodzenie nie może być większe niż Maksymalne wynagrodzenie.");

            var exists = await _context.CostLimits.AnyAsync(c => c.RangeOfKmMin < dto.RangeOfKmMax && c.RangeOfKmMax > dto.RangeOfKmMin);
            if (exists)
                throw new InvalidOperationException("Limit dla podanego zakresu kilometrów już istnieje.");

            var cost = new CostLimit
            {
                RangeOfKmMin = dto.RangeOfKmMin,
                RangeOfKmMax = dto.RangeOfKmMax,
                BaseSalary = dto.BaseSalary,
                MaxSalary = dto.MaxSalary,
                MaxCosts = dto.MaxCosts
            };

            _context.CostLimits.Add(cost);
            await _context.SaveChangesAsync();

            return cost.ToResponseDto();
        }

        public async Task<CostLimitResponseDto> Update(int id, CostLimitUpdateDto dto)
        {
            var c = await _context.CostLimits.FindAsync(id)
                ?? throw new ArgumentException("Nie znaleziono limitu kosztów.");

            var finalMin = dto.RangeOfKmMin ?? c.RangeOfKmMin;
            var finalMax = dto.RangeOfKmMax ?? c.RangeOfKmMax;
            if (finalMin <= 0 || finalMax <= 0)
                throw new InvalidOperationException("Zasięg musi być większy od 0.");

            if (finalMin >= finalMax)
                throw new InvalidOperationException("Początek zakresu musi być mniejszy niż Koniec.");

            var exists = await _context.CostLimits.AnyAsync(c => c.RangeOfKmMin < finalMax && c.RangeOfKmMax > finalMin && c.Id != id);
            if (exists)
                throw new InvalidOperationException("Limit dla podanego zakresu kilometrów już istnieje.");

            c.RangeOfKmMax = finalMax;
            c.RangeOfKmMin = finalMin;

            if (dto.BaseSalary is not null)
                if (dto.BaseSalary <= 0)
                    throw new InvalidOperationException("Podstawowe wynagrodzenie musi być większe od 0.");
                else
                    c.BaseSalary = dto.BaseSalary.Value;

            if (dto.MaxSalary is not null)
                if (dto.MaxSalary <= 0)
                    throw new InvalidOperationException("Maksymalne wynagrodzenie musi być większe od 0.");
                else
                    c.MaxSalary = dto.MaxSalary.Value;

            if (c.BaseSalary > c.MaxSalary)
                throw new InvalidOperationException("Podstawowe wynagrodzenie nie może być większe niż Maksymalne wynagrodzenie.");

            if (dto.MaxCosts is not null)
                if (dto.MaxCosts <= 0)
                    throw new InvalidOperationException("Maksymalne koszty muszą być większe od 0.");
                else
                    c.MaxCosts = dto.MaxCosts.Value;

            c.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return c.ToResponseDto();
        }

        public async Task<bool> Delete(int id)
        {
            var c = await _context.CostLimits.FindAsync(id)
                ?? throw new ArgumentException("Nie znaleziono limitu kosztów.");

            _context.CostLimits.Remove(c);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
