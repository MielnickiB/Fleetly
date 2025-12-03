using Fleetly.Shared.Dto.VehicleDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Helpers;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.VehicleService
{
    public class VehicleService(FleetlyContext context) : IVehicleService
    {
        private readonly FleetlyContext _context = context;

        public async Task<List<VehicleResponseDto>> GetAll(int page, int pageSize)
        {
            var (skip, safe) = PaginationHelper.Calculate(page, pageSize);

            return await _context.Vehicles
                .AsNoTracking()
                .Include(v => v.BrandModel)
                .OrderBy(v => v.Id)
                .Skip(skip)
                .Take(safe)
                .Select(v => v.ToResponseDto())
                .ToListAsync();
        }

        public async Task<List<VehicleResponseDto>> GetUserVehicles(int userId)
        {
            return await _context.Vehicles
                .AsNoTracking()
                .Where(v => v.UserId == userId)
                .Include(v => v.BrandModel)
                .Select(v => v.ToResponseDto())
                .ToListAsync();
        }

        public async Task<VehicleResponseDto?> GetById(int id)
        {
            var v = await _context.Vehicles
                .Include(v => v.BrandModel)
                .FirstOrDefaultAsync(v => v.Id == id);

            return v?.ToResponseDto();
        }

        public async Task<VehicleResponseDto> Create(int userId, VehicleCreateDto dto)
        {
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
                throw new ArgumentException("Podany BrandModelId nie istnieje.");

            var vehicle = new Vehicle
            {
                UserId = userId,
                BrandModelId = dto.BrandModelId,
                RegistrationNumber = dto.RegistrationNumber,
                Mileage = dto.Mileage,
                VIN = dto.VIN,
                Year = dto.Year,
                Details = dto.Details
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            await _context.Entry(vehicle).Reference(v => v.BrandModel).LoadAsync();

            return vehicle.ToResponseDto();
        }

        public async Task<VehicleResponseDto> Update(int id, VehicleUpdateDto dto, int? userId = null)
        {
            var v = await _context.Vehicles
                .Include(v => v.BrandModel)
                .FirstOrDefaultAsync(v => v.Id == id)
                ?? throw new ArgumentException("Nie znaleziono pojazdu.");

            if (userId.HasValue && v.UserId != userId.Value)
                throw new UnauthorizedAccessException("Nie masz uprawnień do edycji tego pojazdu.");

            if (!v.IsActive)
                throw new InvalidOperationException("Nie można edytować nieaktywnego pojazdu.");

            var exists = false;

            if (dto.RegistrationNumber is not null)
            {
                exists = await _context.Vehicles.AnyAsync(v =>
                    v.RegistrationNumber == dto.RegistrationNumber);
                if (exists)
                    throw new ArgumentException("Pojazd o takim numerze rejestracyjnym już istnieje.");
            }

            if (dto.VIN is not null)
            {
                exists = await _context.Vehicles.AnyAsync(v => v.VIN == dto.VIN);
                if (exists)
                    throw new ArgumentException("Pojazd o takim numerze VIN już istnieje.");
            }

            if (dto.BrandModelId.HasValue && dto.BrandModelId.Value != v.BrandModelId)
            {
                var brandExists = await _context.BrandModels.AnyAsync(b => b.Id == dto.BrandModelId.Value);
                if (!brandExists)
                    throw new ArgumentException("Podany BrandModelId nie istnieje.");

                v.BrandModelId = dto.BrandModelId.Value;
            }
            if (dto.RegistrationNumber is not null && dto.RegistrationNumber != v.RegistrationNumber) v.RegistrationNumber = dto.RegistrationNumber;
            if (dto.Mileage.HasValue && dto.Mileage.Value != v.Mileage) v.Mileage = dto.Mileage.Value;
            if (dto.VIN is not null && dto.VIN != v.VIN) v.VIN = dto.VIN;
            if (dto.Year.HasValue && dto.Year.Value != v.Year) v.Year = dto.Year.Value;
            if (dto.Details is not null && dto.Details != v.Details) v.Details = dto.Details;

            v.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _context.Entry(v).Reference(v => v.BrandModel).LoadAsync();

            return v.ToResponseDto();
        }

        public async Task<bool> Deactivate(int id, int? userId = null)
        {
            var v = await _context.Vehicles.FindAsync(id)
                ?? throw new ArgumentException("Nie znaleziono pojazdu.");

            if (userId.HasValue && v.UserId != userId.Value)
                throw new UnauthorizedAccessException("Nie masz uprawnień do usunięcia tego pojazdu.");

            if (!v.IsActive)
                throw new InvalidOperationException("Pojazd jest już nieaktywny.");

            v.IsActive = false;
            v.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
