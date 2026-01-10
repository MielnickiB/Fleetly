using Fleetly.Shared.Dto.DamageDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using FleetlyBackend.Services.FileService;
using Microsoft.EntityFrameworkCore;


namespace FleetlyBackend.Services.DamageService
{
    public class DamageService(FleetlyContext context, IFileService fileService)
        : IDamageService
    {
        private readonly FleetlyContext _context = context;
        private readonly IFileService _fileService = fileService;

        public async Task<List<DamageResponseDto>> GetForVehicle(int vehicleId)
        {
            if (!await _context.Vehicles.AnyAsync(v => v.Id == vehicleId))
                throw new ArgumentException("Podany pojazd nie istnieje.");

            return await _context.Damages
                .AsNoTracking()
                .Where(d => d.VehicleId == vehicleId)
                .OrderBy(d => d.Id)
                .Select(d => d.ToResponseDto())
                .ToListAsync();
        }

        public async Task<DamageResponseDto?> Get(int id)
        {
            var damage = await _context.Damages.FindAsync(id);
            return damage?.ToResponseDto();
        }

        public async Task<DamageResponseDto> Create(DamageCreateDto dto)
        {
            if (!await _context.Vehicles.AnyAsync(v => v.Id == dto.VehicleId))
                throw new ArgumentException("Podany pojazd nie istnieje.");

            if (!await _context.Protocols.AnyAsync(p => p.Id == dto.ProtocolId))
                throw new ArgumentException("Podany protokół nie istnieje.");

            string? fileName = null;
            try
            {
                fileName = await _fileService.SaveFileAsync(dto.Photo, "Vehicle");

                var damage = new Damage
                {
                    VehicleId = dto.VehicleId,
                    ProtocolId = dto.ProtocolId,
                    DamageSide = dto.DamageSide,
                    DamageLocation = dto.DamageLocation,
                    DamagePart = dto.DamagePart,
                    DamageType = dto.DamageType,
                    Description = dto.Description,
                    PhotoUrl = fileName
                };

                _context.Damages.Add(damage);
                try { await _context.SaveChangesAsync(); }
                catch
                {
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        try { await _fileService.DeleteFileAsync(fileName); }
                        catch (InvalidOperationException ex) { throw new InvalidOperationException("Wystąpił błąd podczas usuwania zdjęcia uszkodzenia: " + ex.Message); }
                    }
                    throw;
                }

                return damage.ToResponseDto();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Wystąpił błąd podczas zapisu zdjęcia uszkodzenia: " + ex.Message);
            }
        }

        public async Task<DamageResponseDto> Update(int id, DamageUpdateDto dto)
        {
            var damage = await _context.Damages.FindAsync(id)
                ?? throw new ArgumentException("Nie znaleziono uszkodzenia.");

            var oldFile = damage.PhotoUrl;
            string? newFile = null;

            if (dto.DamageLocation is not null)
                damage.DamageLocation = dto.DamageLocation;

            if (dto.DamageSide is not null)
                damage.DamageSide = dto.DamageSide;

            if (dto.DamagePart is not null)
                damage.DamagePart = dto.DamagePart;

            if (dto.DamageType is not null)
                damage.DamageType = dto.DamageType;

            if (dto.Description is not null)
                damage.Description = dto.Description;

            if (dto.Photo is not null)
            {
                try
                {
                    newFile = await _fileService.SaveFileAsync(dto.Photo, "Vehicle");
                    damage.PhotoUrl = newFile;
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException("Wystąpił błąd podczas aktualizacji zdjęcia uszkodzenia: " + ex.Message);
                }

            }

            damage.UpdatedAt = DateTime.UtcNow;

            try { await _context.SaveChangesAsync(); }
            catch
            {
                if (!string.IsNullOrEmpty(newFile))
                {
                    try { await _fileService.DeleteFileAsync(newFile); }
                    catch (InvalidOperationException ex) { 
                        damage.PhotoUrl = oldFile;
                        throw new InvalidOperationException("Wystąpił błąd podczas usuwania zdjęcia uszkodzenia: " + ex.Message); 
                    }
                }
                throw;
            }

            if (!string.IsNullOrEmpty(newFile) && !string.IsNullOrEmpty(oldFile) && oldFile != newFile)
            {
                try { await _fileService.DeleteFileAsync(oldFile); }
                catch (InvalidOperationException ex) { throw new InvalidOperationException("Wystąpił błąd podczas usuwania starego zdjęcia uszkodzenia: " + ex.Message); }
            }

            return damage.ToResponseDto();
        }

        public async Task<bool> Delete(int id)
        {
            var damage = await _context.Damages.FindAsync(id) ?? throw new ArgumentException("Nie znaleziono uszkodzenia.");

            var fileToDelete = damage.PhotoUrl;

            _context.Damages.Remove(damage);

            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(fileToDelete))
            {
                try { await _fileService.DeleteFileAsync(fileToDelete); }
                catch (InvalidOperationException ex) { throw new InvalidOperationException("Wystąpił błąd podczas usuwania zdjęcia uszkodzenia: " + ex.Message); }
            }

            return true;
        }
    }
}
