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

        public async Task<List<AvailabilityResponseDto>> GetAll(int page, int pageSize)
        {
            var (skip, take) = PaginationHelper.Calculate(page, pageSize);
            var user = _http.CurrentUser();

            var query = _context.Availabilities.AsNoTracking();

            if (user.IsWorker())
            {
                var userId = user.GetUserId();
                query = query.Where(a => a.WorkerId == userId);
            }

            return await query
                .OrderBy(a => a.StartDate)
                .ThenBy(a => a.StartHour)
                .Skip(skip)
                .Take(take)
                .Select(a => a.ToResponseDto())
                .ToListAsync();
        }

        public async Task<AvailabilityResponseDto?> Get(int id)
        {
            var user = _http.CurrentUser();
            var availability = await _context.Availabilities.FindAsync(id);
            if (availability is null)
                return null;
            if (user.IsWorker() && user.GetUserId() != availability.WorkerId)
                throw new UnauthorizedAccessException("Nie masz uprawnień do przeglądania tej dostępności.");
            return availability.ToResponseDto();
        }

        public async Task<AvailabilityResponseDto> Create(AvailabilityCreateDto dto)
        {
            var user = _http.CurrentUser();
            var workerId = user.GetUserId();

            if (dto.EndDate < dto.StartDate)
                throw new InvalidOperationException("Data zakończenia dostępności musi być równa lub późniejsza niż data rozpoczęcia.");

            if (dto.StartDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
                throw new InvalidOperationException("Data dostępności nie może zaczynać się w przeszłości.");

            var startHour = dto.StartHour;
            var endHour = dto.EndHour;
            if (endHour <= startHour)
                throw new InvalidOperationException("Godzina zakończenia musi być późniejsza niż godzina rozpoczęcia.");

            var availabilityExists = await _context.Availabilities.AnyAsync(a =>
                a.WorkerId == workerId
                && a.StartDate <= dto.EndDate
                && a.EndDate >= dto.StartDate
                && startHour < a.EndHour
                && a.StartHour < endHour);

            if (availabilityExists)
                throw new InvalidOperationException("W podanym zakresie dat i godzin istnieje już dostępność.");

            var entity = new Availability
            {
                WorkerId = workerId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                StartHour = startHour,
                EndHour = endHour,
                IsAvailable = dto.IsAvailable
            };

            _context.Availabilities.Add(entity);
            await _context.SaveChangesAsync();

            return entity.ToResponseDto();
        }

        public async Task<AvailabilityResponseDto> Update(int id, AvailabilityUpdateDto dto)
        {
            var user = _http.CurrentUser();

            var entity = await _context.Availabilities.FindAsync(id)
                ?? throw new ArgumentException("Nie znaleziono dostępności.");

            if (user.GetUserId() != entity.WorkerId)
                throw new UnauthorizedAccessException("Nie masz uprawnień do edytowania tej dostępności.");

            var newStartDate = dto.StartDate ?? entity.StartDate;
            var newEndDate = dto.EndDate ?? entity.EndDate;

            if (newEndDate < newStartDate)
                throw new ArgumentException("Data zakończenia dostępności musi być równa lub późniejsza niż data rozpoczęcia.");

            if (newStartDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
                throw new InvalidOperationException("Data dostępności nie może zaczynać się w przeszłości.");

            var newStartHour = dto.StartHour ?? entity.StartHour;
            var newEndHour = dto.EndHour ?? entity.EndHour;

            if (newEndHour <= newStartHour)
                throw new InvalidOperationException("Godzina zakończenia musi być późniejsza niż godzina rozpoczęcia.");

            var availabilityExists = await _context.Availabilities.AnyAsync(a =>
                a.Id != id
                && a.WorkerId == entity.WorkerId
                && a.StartDate <= newEndDate
                && a.EndDate >= newStartDate
                && newStartHour < a.EndHour
                && a.StartHour < newEndHour);

            if (availabilityExists)
                throw new ArgumentException("W podanym zakresie dat i godzin istnieje już inna dostępność.");

            if (dto.IsAvailable.HasValue) entity.IsAvailable = dto.IsAvailable.Value;
            entity.StartDate = newStartDate;
            entity.EndDate = newEndDate;
            entity.StartHour = newStartHour;
            entity.EndHour = newEndHour;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return entity.ToResponseDto();
        }

        public async Task<bool> Delete(int id)
        {
            var user = _http.CurrentUser();

            var entity = await _context.Availabilities.FindAsync(id) ?? throw new ArgumentException("Nie znaleziono dostępności.");
            if (user.GetUserId() != entity.WorkerId)
                throw new UnauthorizedAccessException("Nie masz uprawnień do usunięcia tej dostępności.");

            _context.Availabilities.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}