using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.LocationDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Extensions;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.LocationService
{
    public class LocationService(FleetlyContext context, IHttpContextAccessor http) : ILocationService
    {
        private readonly FleetlyContext _context = context;
        private readonly IHttpContextAccessor _http = http;

        public async Task<PagedResult<LocationResponseDto>> GetAll(bool includeInactive)
        {
            var user = _http.CurrentUser();
            var query = _context.Locations.AsNoTracking().AsQueryable();

            if (user.IsClient() || user.IsWorker())
            {
                query = query.Where(l => l.UserId == user.GetUserId());
            }

            if (!includeInactive)
            {
                query = query.Where(l => l.IsActive);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(l => l.Id)
                .Include(v => v.User).ThenInclude(u => u.Details)
                .Select(l => l.ToLocationResponseDto())
                .ToListAsync();

            return new PagedResult<LocationResponseDto>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<LocationResponseDto?> GetById(int id)
        {
            var user = _http.CurrentUser();
            var loc = await _context.Locations.FindAsync(id);
            if (loc is null)
                return null;
            if ((user.IsClient() || user.IsWorker()) && user.GetUserId() != loc.UserId)
                throw new UnauthorizedAccessException("Nie masz uprawnień do przeglądania tej lokalizacji.");
            return loc.ToLocationResponseDto();
        }

        public async Task<LocationResponseDto> Create(LocationCreateDto dto)
        {
            var currentUserId = _http.CurrentUser().GetUserId();
            var location = new Location
            {
                UserId = currentUserId,
                City = dto.City,
                Street = dto.Street,
                BuildingNumber = dto.BuildingNumber,
                ApartmentNumber = dto.ApartmentNumber,
                PostalCode = dto.PostalCode,
                IsPublic = dto.IsPublic,
                Description = dto.Description
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return await GetFresh(location.Id);
        }

        public async Task<LocationResponseDto> Update(int id, LocationUpdateDto dto)
        {
            var user = _http.CurrentUser();
            var userId = user.GetUserId();

            var loc = await _context.Locations.FindAsync(id);
            if (loc is null || loc.UserId != userId && (user.IsClient() || user.IsWorker()))
                throw new UnauthorizedAccessException("Nie możesz modyfikować cudzej lokalizacji.");
            if (!loc.IsActive) throw new InvalidOperationException("Nie można modyfikować usuniętej lokalizacji.");


            if (!string.IsNullOrWhiteSpace(dto.City)) loc.City = dto.City;
            if (!string.IsNullOrWhiteSpace(dto.Street)) loc.Street = dto.Street;
            if (!string.IsNullOrWhiteSpace(dto.BuildingNumber)) loc.BuildingNumber = dto.BuildingNumber;
            if (!string.IsNullOrWhiteSpace(dto.PostalCode)) loc.PostalCode = dto.PostalCode;

            if (!string.Equals(loc.ApartmentNumber, dto.ApartmentNumber, StringComparison.OrdinalIgnoreCase))
                loc.ApartmentNumber = dto.ApartmentNumber;
            if (!string.Equals(loc.Description, dto.Description, StringComparison.OrdinalIgnoreCase))
                loc.Description = dto.Description;

            loc.IsPublic = dto.IsPublic;

            loc.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetFresh(loc.Id);
        }

        public async Task<bool> Deactivate(int id)
        {
            var user = _http.CurrentUser();
            var userId = user.GetUserId();

            var loc = await _context.Locations.FindAsync(id);

            if (loc is not null && loc.UserId != userId && (user.IsClient() || user.IsWorker()))
                throw new UnauthorizedAccessException("Nie możesz usuwać cudzej lokalizacji.");

            if (loc is null)
                throw new ArgumentException("Nie znaleziono lokalizacji.");

            if (!loc.IsActive)
                throw new InvalidOperationException("Lokalizacja jest już nieaktywna.");

            loc.IsActive = false;
            loc.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<LocationResponseDto> GetFresh(int id)
        {
            var fresh = await _context.Locations
                .AsNoTracking()
                .Include(l => l.User).ThenInclude(l => l.Details)
                .FirstOrDefaultAsync(l => l.Id == id)
                ?? throw new ArgumentException("Nie znaleziono lokalizacji.");

            return fresh.ToLocationResponseDto();
        }
    }
}
