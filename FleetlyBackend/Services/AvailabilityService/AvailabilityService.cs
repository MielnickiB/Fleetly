using Fleetly.Shared.Dto.AvailabilityDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Extensions;
using FleetlyBackend.Helpers;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.AvailabilityService
{
    public class AvailabilityService(FleetlyContext context, IHttpContextAccessor http) : IAvailabilityService
    {
        private readonly FleetlyContext _context = context;
        private readonly IHttpContextAccessor _http = http;

        public async Task<List<AvailabilityResponseDto>> GetByRange(DateOnly start, DateOnly end)
        {
            var user = _http.CurrentUser();

            var query = _context.Availabilities.AsNoTracking().AsQueryable();

            if (user.IsWorker())
            {
                var userId = user.GetUserId();
                query = query.Where(a => a.WorkerId == userId);
            }

            return await query
                .Where(a => a.Date >= start && a.Date <= end)
                .OrderBy(a => a.Date)
                .ThenBy(a => a.StartHour)
                .Select(a => a.ToResponseDto())
                .ToListAsync();
        }

        public async Task<AvailabilityResponseDto?> Get(int id)
        {
            var user = _http.CurrentUser();
            var availability = await _context.Availabilities.FindAsync(id);

            if (availability is null) return null;

            if (user.IsWorker() && user.GetUserId() != availability.WorkerId)
                throw new UnauthorizedAccessException("Nie masz uprawnień do przeglądania tej dostępności.");

            return availability.ToResponseDto();
        }

        public async Task<List<AvailabilityResponseDto>> Create(AvailabilityCreateDto dto)
        {
            var user = _http.CurrentUser();
            var workerId = user.GetUserId();

            if (dto.StartDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
                throw new InvalidOperationException("Nie można dodać dostępności w przeszłości.");

            if (dto.EndDate < dto.StartDate)
                throw new InvalidOperationException("Data końcowa nie może być wcześniejsza niż początkowa.");

            if (dto.EndHour <= dto.StartHour)
                throw new InvalidOperationException("Godzina zakończenia musi być późniejsza niż rozpoczęcia.");

            var existingAvailabilities = await _context.Availabilities
                .Where(a => a.WorkerId == workerId && a.Date >= dto.StartDate && a.Date <= dto.EndDate)
                .ToListAsync();

            var newEntities = new List<Availability>();

            for (var dt = dto.StartDate; dt <= dto.EndDate; dt = dt.AddDays(1))
            {
                var conflict = existingAvailabilities
                    .Any(e => e.Date == dt && (e.StartHour < dto.EndHour && dto.StartHour < e.EndHour));

                if (conflict)
                    throw new InvalidOperationException($"Konflikt dostępności w dniu {dt}. Zmień zakres lub usuń istniejący wpis.");

                newEntities.Add(new Availability
                {
                    WorkerId = workerId,
                    Date = dt,
                    StartHour = dto.StartHour,
                    EndHour = dto.EndHour,
                    IsAvailable = dto.IsAvailable
                });
            }

            if (newEntities.Count != 0)
            {
                _context.Availabilities.AddRange(newEntities);
                await _context.SaveChangesAsync();
            }

            return newEntities.Select(x => x.ToResponseDto()).ToList();
        }

        public async Task<AvailabilityResponseDto> Update(int id, AvailabilityUpdateDto dto)
        {
            var entity = await _context.Availabilities.FindAsync(id)
                ?? throw new ArgumentException("Nie znaleziono dostępności.");

            var user = _http.CurrentUser();
            if (user.GetUserId() != entity.WorkerId)
                throw new UnauthorizedAccessException("Nie masz uprawnień do edytowania tej dostępności.");

            var newStart = dto.StartHour ?? entity.StartHour;
            var newEnd = dto.EndHour ?? entity.EndHour;

            if (newEnd <= newStart)
                throw new InvalidOperationException("Godzina zakończenia musi być późniejsza niż rozpoczęcia.");

            var hasConflict = await _context.Availabilities.AnyAsync(a =>
                a.Id != id &&
                a.WorkerId == entity.WorkerId &&
                a.Date == entity.Date && 
                (a.StartHour < newEnd && newStart < a.EndHour));

            if (hasConflict)
                throw new InvalidOperationException("W tym dniu masz już inną dostępność w tych godzinach.");

            entity.StartHour = newStart;
            entity.EndHour = newEnd;

            if (dto.IsAvailable.HasValue)
                entity.IsAvailable = dto.IsAvailable.Value;

            await _context.SaveChangesAsync();
            return entity.ToResponseDto();
        }

        public async Task Delete(int id)
        {
            var user = _http.CurrentUser();

            var entity = await _context.Availabilities.FindAsync(id)
                ?? throw new ArgumentException("Nie znaleziono dostępności.");

            if (user.GetUserId() != entity.WorkerId)
                throw new UnauthorizedAccessException("Brak uprawnień.");

            _context.Availabilities.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}