using Fleetly.Shared.Dto.LocationDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Helpers;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.LocationService
{
    public class LocationService(FleetlyContext context) : ILocationService
    {
        private readonly FleetlyContext _context = context;

        public async Task<List<LocationResponseDto>> GetAll(int page, int pageSize)
        {
            var (skip, safe) = PaginationHelper.Calculate(page, pageSize);

            return await _context.Locations
                .AsNoTracking()
                .OrderBy(l => l.Id)
                .Skip(skip)
                .Take(safe)
                .Select(l => l.ToLocationResponseDto())
                .ToListAsync();
        }

        public async Task<List<LocationResponseDto>> GetUserLocations(int userId)
        {
            return await _context.Locations
                .AsNoTracking()
                .Where(l => l.UserId == userId)
                .OrderBy(l => l.Id)
                .Select(l => l.ToLocationResponseDto())
                .ToListAsync();
        }

        public async Task<LocationResponseDto?> GetById(int id)
        {
            var loc = await _context.Locations.FindAsync(id);
            return loc?.ToLocationResponseDto();
        }

        public async Task<LocationResponseDto> Create(LocationCreateDto dto, int currentUserId)
        {
            var location = new Location
            {
                UserId = currentUserId,
                City = dto.City,
                Street = dto.Street,
                BuildingNumber = dto.BuildingNumber,
                ApartmentNumber = dto.ApartmentNumber,
                PostalCode = dto.PostalCode
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return location.ToLocationResponseDto();
        }

        public async Task<LocationResponseDto> Update(int id, LocationUpdateDto dto, int? currentUserId = null)
        {
            var loc = await _context.Locations.FindAsync(id)
                ?? throw new ArgumentException("Nie znaleziono lokalizacji.");

            if (!loc.IsActive)
                throw new ArgumentException("Nie można modyfikować usuniętej lokalizacji.");

            if (currentUserId != null && loc.UserId != currentUserId)
                throw new UnauthorizedAccessException("Nie możesz modyfikować cudzej lokalizacji.");

            if (dto.City is not null && dto.City != loc.City) loc.City = dto.City;
            if (dto.Street is not null && dto.Street != loc.Street) loc.Street = dto.Street;
            if (dto.BuildingNumber is not null && dto.BuildingNumber != loc.BuildingNumber) loc.BuildingNumber = dto.BuildingNumber;
            if (dto.ApartmentNumber is not null && dto.ApartmentNumber != loc.ApartmentNumber) loc.ApartmentNumber = dto.ApartmentNumber;
            if (dto.PostalCode is not null && dto.PostalCode != loc.PostalCode) loc.PostalCode = dto.PostalCode;

            loc.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return loc.ToLocationResponseDto();
        }

        public async Task<bool> Deactivate(int id, int? currentUserId = null)
        {
            var loc = await _context.Locations.FindAsync(id)
                ?? throw new ArgumentException("Nie znaleziono lokalizacji.");

            if (!loc.IsActive)
                throw new InvalidOperationException("Lokalizacja jest już nieaktywna.");

            if (currentUserId != null && loc.UserId != currentUserId)
                throw new UnauthorizedAccessException("Nie możesz usunąć cudzej lokalizacji.");

            loc.IsActive = false;
            loc.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
