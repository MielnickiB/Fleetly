using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.LocationDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Extensions;
using FleetlyBackend.Helpers;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.LocationService
{
    public class LocationService(FleetlyContext context, IHttpContextAccessor http) : ILocationService
    {
        private readonly FleetlyContext _context = context;
        private readonly IHttpContextAccessor _http = http;

        public async Task<PagedResult<LocationResponseDto>> GetAll()
        {
            var user = _http.CurrentUser();
            var query = _context.Locations.AsNoTracking().AsQueryable();

            if (user.IsClient() || user.IsWorker())
            {
                query = query.Where(l => l.UserId == user.GetUserId());
            }

            query = query.Where(l => l.IsPublic);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(l => l.Id)
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

            return location.ToLocationResponseDto();
        }

        public async Task<LocationResponseDto> Update(int id, LocationUpdateDto dto)
        {
            var user = _http.CurrentUser();
            var userId = user.GetUserId();

            var loc = await _context.Locations.FindAsync(id);

            if (loc is not null && loc.UserId != userId && (user.IsClient() || user.IsWorker()))
                throw new UnauthorizedAccessException("Nie możesz modyfikować cudzej lokalizacji.");

            if (loc is null)
                throw new ArgumentException("Nie znaleziono lokalizacji.");

            if (!loc.IsActive)
                throw new InvalidOperationException("Nie można modyfikować usuniętej lokalizacji.");

            if (dto.City is not null && dto.City != string.Empty && !dto.City.Equals(loc.City, StringComparison.OrdinalIgnoreCase)) loc.City = dto.City;
            if (dto.Street is not null && dto.Street != string.Empty && !dto.Street.Equals(loc.Street, StringComparison.OrdinalIgnoreCase)) loc.Street = dto.Street;
            if (dto.BuildingNumber is not null && dto.BuildingNumber != string.Empty && !dto.BuildingNumber.Equals(loc.BuildingNumber, StringComparison.OrdinalIgnoreCase)) loc.BuildingNumber = dto.BuildingNumber;
            if (dto.ApartmentNumber is not null && dto.ApartmentNumber != string.Empty && !dto.ApartmentNumber.Equals(loc.ApartmentNumber, StringComparison.OrdinalIgnoreCase)) loc.ApartmentNumber = dto.ApartmentNumber;
            if (dto.PostalCode is not null && dto.PostalCode != string.Empty && !dto.PostalCode.Equals(loc.PostalCode, StringComparison.OrdinalIgnoreCase)) loc.PostalCode = dto.PostalCode;
            if (dto.IsPublic != loc.IsPublic) loc.IsPublic = dto.IsPublic;
            if (dto.Description is not null && dto.Description != string.Empty && !dto.Description.Equals(loc.Description, StringComparison.OrdinalIgnoreCase)) loc.Description = dto.Description;

            loc.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return loc.ToLocationResponseDto();
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
    }
}
