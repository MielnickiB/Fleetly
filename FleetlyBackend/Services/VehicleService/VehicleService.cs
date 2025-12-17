using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.VehicleDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Extensions;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.VehicleService
{
    public class VehicleService(FleetlyContext context, IHttpContextAccessor http) : IVehicleService
    {
        private readonly FleetlyContext _context = context;
        private readonly IHttpContextAccessor _http = http;

        public async Task<PagedResult<VehicleResponseDto>> GetAll()
        {
            var user = _http.CurrentUser();
            var userId = user.GetUserId();

            var vehiclesQuery = _context.Vehicles
                .AsNoTracking()
                .Include(v => v.BrandModel).ThenInclude(bm => bm.CarBrand)
                .Include(v => v.User).ThenInclude(u => u.Details)
                .AsQueryable();

            if (!user.IsInRole("Admin"))
            {
                vehiclesQuery = vehiclesQuery.Where(v => v.UserId == userId);
            }

            var totalCount = await vehiclesQuery.CountAsync();

            var vehicles = await vehiclesQuery
                .Select(v => v.ToResponseDto())
                .ToListAsync();

            return new PagedResult<VehicleResponseDto>
            {
                Items = vehicles,
                TotalCount = totalCount
            };
        }

        public async Task<VehicleResponseDto?> GetById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Nieprawidłowe ID pojazdu.");

            var user = _http.CurrentUser();
            var userId = user.GetUserId();

            var v = await _context.Vehicles
                .AsNoTracking()
                .Include(v => v.BrandModel).ThenInclude(bm => bm.CarBrand)
                .Include(v => v.User).ThenInclude(u => u.Details)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (v is null)
                return null;

            if (!user.IsInRole("Admin") && v.UserId != userId)
                throw new UnauthorizedAccessException("Nie masz uprawnień do przeglądania tego pojazdu.");

            return v.ToResponseDto();
        }

        public async Task<VehicleResponseDto> Create(VehicleCreateDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var userId = _http.CurrentUser().GetUserId();

            var exists = await _context.Vehicles.AnyAsync(v =>
                v.RegistrationNumber == dto.RegistrationNumber);

            if (exists)
                throw new ArgumentException("Pojazd o takim numerze rejestracyjnym już istnieje.");

            if (dto.VIN is not null)
            {
                exists = await _context.Vehicles.AnyAsync(v => v.VIN == dto.VIN);
                if (exists)
                    throw new ArgumentException("Pojazd o takim numerze VIN już istnieje.");
            }

            var brandExists = await _context.BrandModels.AnyAsync(b => b.Id == dto.BrandModelId);
            if (!brandExists)
                throw new ArgumentException("Podany Model nie istnieje.");

            var vehicle = new Vehicle
            {
                UserId = userId,
                BrandModelId = dto.BrandModelId,
                RegistrationNumber = dto.RegistrationNumber.ToUpper(),
                Mileage = dto.Mileage,
                VIN = dto.VIN,
                Year = dto.Year,
                Details = dto.Details,
                FuelType = dto.FuelType,
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            var created = await _context.Vehicles
                .AsNoTracking()
                .Include(v => v.BrandModel).ThenInclude(bm => bm.CarBrand)
                .Include(v => v.User).ThenInclude(u => u.Details)
                .FirstOrDefaultAsync(v => v.Id == vehicle.Id)
                ?? throw new InvalidOperationException("Nie udało się pobrać utworzonego pojazdu.");

            return created.ToResponseDto();
        }

        public async Task<VehicleResponseDto> Update(int id, VehicleUpdateDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if (id <= 0)
                throw new ArgumentException("Nieprawidłowe ID pojazdu.");

            var user = _http.CurrentUser();
            var userId = user.GetUserId();

            var v = await _context.Vehicles
                .Include(v => v.BrandModel).ThenInclude(bm => bm.CarBrand)
                .Include(v => v.User).ThenInclude(u => u.Details)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (v is not null && userId != v.UserId && !user.IsInRole("Admin"))
                throw new UnauthorizedAccessException("Nie masz uprawnień do edycji tego pojazdu.");

            if (v is null)
                throw new ArgumentException("Nie znaleziono pojazdu.");

            if (!v.IsActive)
                throw new InvalidOperationException("Nie można edytować nieaktywnego pojazdu.");

            var exists = false;

            if (dto.RegistrationNumber is not null)
            {
                exists = await _context.Vehicles.AnyAsync(v =>
                    v.RegistrationNumber == dto.RegistrationNumber);
                if (exists)
                    throw new ArgumentException("Pojazd o takim numerze rejestracyjnym już istnieje.");
                v.RegistrationNumber = dto.RegistrationNumber;
            }

            if (dto.VIN is not null)
            {
                exists = await _context.Vehicles.AnyAsync(v => v.VIN == dto.VIN);
                if (exists)
                    throw new ArgumentException("Pojazd o takim numerze VIN już istnieje.");
                v.VIN = dto.VIN;
            }

            if (dto.BrandModelId.HasValue && dto.BrandModelId.Value != v.BrandModelId)
            {
                var brandExists = await _context.BrandModels.AnyAsync(b => b.Id == dto.BrandModelId.Value);
                if (!brandExists)
                    throw new ArgumentException("Podany model nie istnieje.");

                v.BrandModelId = dto.BrandModelId.Value;
            }

            if (dto.Mileage.HasValue && dto.Mileage.Value != v.Mileage) v.Mileage = dto.Mileage.Value;
            if (dto.Year.HasValue && dto.Year.Value != v.Year) v.Year = dto.Year.Value;
            if (dto.Details is not null && dto.Details != v.Details) v.Details = dto.Details;

            v.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            var updated = await _context.Vehicles
                .AsNoTracking()
                .Include(ve => ve.BrandModel).ThenInclude(bm => bm.CarBrand)
                .Include(ve => ve.User).ThenInclude(u => u.Details)
                .FirstOrDefaultAsync(ve => ve.Id == v.Id)
                ?? throw new InvalidOperationException("Nie udało się pobrać zaktualizowanego pojazdu.");

            return updated.ToResponseDto();
        }

        public async Task<bool> Deactivate(int id)
        {
            var user = _http.CurrentUser();
            var userId = user.GetUserId();

            var v = await _context.Vehicles.FindAsync(id);

            if (v is not null && !user.IsInRole("Admin") && v.UserId != userId)
                throw new UnauthorizedAccessException("Nie masz uprawnień do usunięcia tego pojazdu.");

            if (v is null)
                throw new ArgumentException("Nie znaleziono pojazdu.");

            if (!v.IsActive)
                throw new InvalidOperationException("Pojazd jest już nieaktywny.");

            v.IsActive = false;
            v.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}