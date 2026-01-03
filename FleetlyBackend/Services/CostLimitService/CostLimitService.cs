using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.CostLimitDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.CostLimitService
{
    public class CostLimitService(FleetlyContext context) : ICostLimitService
    {
        private readonly FleetlyContext _context = context;

        public async Task<PagedResult<CostLimitResponseDto>> GetAll()
        {
            var totalCount = await _context.CostLimits.CountAsync();
            var items = await _context.CostLimits
                .AsNoTracking()
                .OrderBy(c => c.RangeOfKmMin)
                .Select(c => c.ToResponseDto())
                .ToListAsync();
            return new PagedResult<CostLimitResponseDto>
            {
                Items = items,
                TotalCount = totalCount
            };
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
            ValidateLogicalRules(dto.RangeOfKmMin, dto.RangeOfKmMax, dto.BaseSalary, dto.MaxSalary);

            await ValidateTimelineContinuity(dto.RangeOfKmMin, dto.RangeOfKmMax);

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

            ValidateLogicalRules(dto.RangeOfKmMin, dto.RangeOfKmMax, dto.BaseSalary, dto.MaxSalary);

            if (c.RangeOfKmMin != dto.RangeOfKmMin || c.RangeOfKmMax != dto.RangeOfKmMax)
            {
                await ValidateTimelineContinuity(dto.RangeOfKmMin, dto.RangeOfKmMax, excludeId: id);
            }

            c.RangeOfKmMin = dto.RangeOfKmMin;
            c.RangeOfKmMax = dto.RangeOfKmMax;
            c.BaseSalary = dto.BaseSalary;
            c.MaxSalary = dto.MaxSalary;
            c.MaxCosts = dto.MaxCosts;
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

        private static void ValidateLogicalRules(int minKm, int maxKm, decimal baseSalary, decimal maxSalary)
        {
            if (minKm >= maxKm)
                throw new InvalidOperationException("Minimalny dystans musi być mniejszy niż maksymalny.");

            if (baseSalary > maxSalary)
                throw new InvalidOperationException("Podstawowe wynagrodzenie nie może być większe niż maksymalne.");
        }

        private async Task ValidateTimelineContinuity(int newMin, int newMax, int? excludeId = null)
        {
            var existingLimits = await _context.CostLimits
                .AsNoTracking()
                .Where(x => !excludeId.HasValue || x.Id != excludeId)
                .OrderBy(x => x.RangeOfKmMin)
                .Select(x => new { x.RangeOfKmMin, x.RangeOfKmMax })
                .ToListAsync();

            var timeline = existingLimits
                .Select(x => (Min: x.RangeOfKmMin, Max: x.RangeOfKmMax))
                .ToList();

            timeline.Add((Min: newMin, Max: newMax));

            timeline = timeline.OrderBy(x => x.Min).ToList();
            for (int i = 0; i < timeline.Count - 1; i++)
            {
                var current = timeline[i];
                var next = timeline[i + 1];

                if (current.Max + 1 < next.Min)
                    throw new InvalidOperationException($"Wykryto dziurę w zakresie kilometrów między {current.Max} a {next.Min}.");

                if (current.Max >= next.Min)
                    throw new InvalidOperationException($"Wykryto nakładanie się zakresów: {current.Max} zachodzi na {next.Min}.");
            }
        }
    }
}
