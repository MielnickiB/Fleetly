using Fleetly.Shared.Dto.DamageDtos;
using Fleetly.Shared.Dto.ProtocolDtos;
using Fleetly.Shared.Enums;
using FleetlyBackend.Data;
using FleetlyBackend.Extensions;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using FleetlyBackend.Services.FileService;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.ProtocolService
{
    public class ProtocolService(FleetlyContext context, IFileService fileService, IHttpContextAccessor http) : IProtocolService
    {

        private readonly FleetlyContext _context = context;
        private readonly IFileService _fileService = fileService;
        private readonly IHttpContextAccessor _http = http;

        public async Task<ProtocolResponseDto?> GetProtocolByOrderIdAsync(int orderId, ProtocolType? type = null)
        {
            var user = _http.CurrentUser();
            var userId = user.GetUserId();

            var order = await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new KeyNotFoundException("Nie znaleziono zlecenia.");

            var targetType = type ?? ((order.Status == OrderStatus.Assigned) ? ProtocolType.Pickup : ProtocolType.Delivery);

            var protocol = await _context.Protocols
                .AsNoTracking()
                .Include(p => p.Order)
                .Include(p => p.Vehicle)
                .Include(p => p.Photos)
                .Include(p => p.Damages)
                .FirstOrDefaultAsync(p => p.OrderId == orderId && p.Type == targetType);

            if (protocol == null) return null;

            if (!user.IsAdmin() && protocol.WorkerId != userId && protocol.ClientId != userId)
                throw new UnauthorizedAccessException("Nie masz dostępu do tego protokołu.");

            return await GetProtocolDtoInternal(protocol.Id);
        }

        public async Task<ProtocolResponseDto> StartProtocolAsync(ProtocolInitDto dto)
        {
            var user = _http.CurrentUser();
            var userId = user.GetUserId();

            var order = await _context.Orders
                .Include(o => o.Vehicle)
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId)
                ?? throw new KeyNotFoundException("Nie znaleziono zlecenia.");

            if (order.WorkerId != userId && !user.IsAdmin())
                throw new UnauthorizedAccessException("Nie jesteś przypisany do tego zlecenia.");

            if (order.Status != OrderStatus.Assigned && order.Status != OrderStatus.OrderStarted && order.StartTime.Date.Equals(DateTime.UtcNow.Date))
                throw new InvalidOperationException("Nie można rozpocząć protokołu dla tego zlecenia w tym momencie.");

            var type = order.Status == OrderStatus.Assigned ? ProtocolType.Pickup : ProtocolType.Delivery;

            var existingProtocol = await _context.Protocols
                .AsNoTracking()
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.OrderId == order.Id && p.Type == type && p.SignatureUrl == null);

            if (existingProtocol != null)
                return await GetProtocolDtoInternal(existingProtocol.Id);

            var protocol = new Protocol
            {
                OrderId = order.Id,
                VehicleId = order.VehicleId,
                WorkerId = userId,
                ClientId = order.ClientId,
                Type = type,
                LocationLatitude = dto.Latitude,
                LocationLongitude = dto.Longitude
            };

            _context.Protocols.Add(protocol);
            await _context.SaveChangesAsync();

            return await GetProtocolDtoInternal(protocol.Id);
        }

        public async Task<ProtocolResponseDto> AddProtocolPhotoAsync(ProtocolPhotoDto dto)
        {
            var protocol = await GetProtocolWithAccessCheck(dto.ProtocolId);

            string folderStructure = Path.Combine("orders", protocol.OrderId.ToString(), "protocols", protocol.Type.ToString(), "photos");

            string fileName = await _fileService.SaveFileAsync(dto.Photo, folderStructure);

            var protocolPhoto = new ProtocolPhoto
            {
                ProtocolId = protocol.Id,
                Side = dto.Side,
                PhotoUrl = fileName
            };

            _context.ProtocolPhotos.Add(protocolPhoto);
            await _context.SaveChangesAsync();

            return await GetProtocolDtoInternal(dto.ProtocolId);
        }

        public async Task<ProtocolResponseDto> AddDamageAsync(DamageCreateDto dto)
        {
            var protocol = await GetProtocolWithAccessCheck(dto.ProtocolId);

            string folderStructure = Path.Combine("vehicles", protocol.Vehicle.RegistrationNumber, "damages");

            string fileName = await _fileService.SaveFileAsync(dto.Photo, folderStructure);

            try
            {
                var damage = new Damage
                {
                    ProtocolId = protocol.Id,
                    VehicleId = protocol.VehicleId,
                    Side = dto.DamageSide,
                    Part = dto.DamagePart,
                    Type = dto.DamageType,
                    Description = dto.Description,
                    PhotoUrl = fileName
                };

                _context.Damages.Add(damage);
                await _context.SaveChangesAsync();

                return await GetProtocolDtoInternal(dto.ProtocolId);
            }
            catch
            {
                await _fileService.DeleteFileAsync(fileName);
                throw;
            }
        }

        public async Task<ProtocolResponseDto> DeleteDamageAsync(int damageId)
        {
            var damage = await _context.Damages
                .Include(d => d.Protocol)
                .FirstOrDefaultAsync(d => d.Id == damageId)
                ?? throw new ArgumentException("Uszkodzenie nie istnieje.");

            if (damage.Protocol == null || damage.ProtocolId == null)
                throw new InvalidOperationException("Uszkodzenie nie jest powiązane z żadnym protokołem.");

            var protocolId = damage.ProtocolId;

            var protocol = await GetProtocolWithAccessCheck((int)protocolId);

            if (protocol.Id != damage.ProtocolId)
                throw new InvalidOperationException("To uszkodzenie nie należy do obecnego protokołu.");

            var user = _http.CurrentUser();

            if (damage.Protocol.WorkerId != user.GetUserId() && !user.IsAdmin())
                throw new UnauthorizedAccessException("Brak dostępu do usunięcia tego uszkodzenia.");

            var fileToDelete = damage.PhotoUrl;
            _context.Damages.Remove(damage);

            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(fileToDelete))
                await _fileService.DeleteFileAsync(fileToDelete);

            return await GetProtocolDtoInternal(protocolId.Value);
        }

        public async Task<ProtocolResponseDto> MarkDamageAsFixedAsync(int damageId, int currentProtocolId)
        {
            var currentProtocol = await GetProtocolWithAccessCheck(currentProtocolId);

            var damage = await _context.Damages
                .FirstOrDefaultAsync(d => d.Id == damageId)
                ?? throw new KeyNotFoundException("Uszkodzenie nie istnieje.");

            if (damage.VehicleId != currentProtocol.VehicleId)
                throw new InvalidOperationException("To uszkodzenie nie dotyczy pojazdu z obecnego protokołu.");

            damage.IsFixed = true;
            damage.FixedAt = DateTime.UtcNow;
            damage.FixedByProtocolId = currentProtocolId;

            await _context.SaveChangesAsync();

            return await GetProtocolDtoInternal(currentProtocolId);
        }

        public async Task<ProtocolResponseDto> FinishProtocolAsync(ProtocolFinishDto dto)
        {
            var protocol = await _context.Protocols
                .Include(p => p.Order)
                .Include(p => p.Vehicle)
                .FirstOrDefaultAsync(p => p.Id == dto.ProtocolId)
                ?? throw new KeyNotFoundException("Protokół nie istnieje.");

            var user = _http.CurrentUser();
            if (protocol.WorkerId != user.GetUserId() && !user.IsAdmin())
                throw new UnauthorizedAccessException("Brak dostępu do protokołu.");

            string folderStructure = Path.Combine("orders", protocol.OrderId.ToString(), "protocols", protocol.Type.ToString(), "signatures");
            string signaturePath = await _fileService.SaveFileAsync(dto.SignaturePhoto, folderStructure);

            protocol.Mileage = dto.Mileage;
            protocol.FuelLevel = dto.FuelLevel;
            protocol.Notes = dto.Notes;
            protocol.HasRegistrationDocument = dto.HasRegistrationDocument;
            protocol.HasServiceBook = dto.HasServiceBook;
            protocol.HasInsurancePolicy = dto.HasInsurancePolicy;
            protocol.NumberOfKeys = dto.NumberOfKeys;
            protocol.SignatureUrl = signaturePath;
            protocol.UpdatedAt = DateTime.UtcNow;

            if (protocol.Type == ProtocolType.Pickup)
            {
                protocol.Order.Status = OrderStatus.OrderStarted;
                protocol.Order.ActualStartTime = DateTime.UtcNow;

                if (dto.Mileage > protocol.Vehicle.Mileage)
                    protocol.Vehicle.Mileage = (int)dto.Mileage;
            }
            else
            {
                protocol.Order.Status = OrderStatus.OrderFinishedByWorker;
                protocol.Order.ActualEndTime = DateTime.UtcNow;

                if (dto.Mileage > protocol.Vehicle.Mileage)
                    protocol.Vehicle.Mileage = (int)dto.Mileage;

            }

            await _context.SaveChangesAsync();

            return await GetProtocolDtoInternal(dto.ProtocolId);
        }

        public async Task UpdateStepAsync(int protocolId, int step)
        {
            var protocol = await GetProtocolWithAccessCheck(protocolId);

            protocol.CurrentStep = step;
            protocol.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        private async Task<Protocol> GetProtocolWithAccessCheck(int protocolId)
        {
            var user = _http.CurrentUser();
            var userId = user.GetUserId();

            var protocol = await _context.Protocols
                .Include(p => p.Order)
                .Include(p => p.Vehicle)
                .Include(p => p.Photos)
                .Include(p => p.Damages)
                .FirstOrDefaultAsync(p => p.Id == protocolId)
                ?? throw new KeyNotFoundException("Protokół nie istnieje.");

            if (protocol.WorkerId != userId && !user.IsAdmin())
                throw new UnauthorizedAccessException("Nie masz uprawnień do edycji tego protokołu.");

            return protocol;
        }

        private async Task<ProtocolResponseDto> GetProtocolDtoInternal(int protocolId)
        {
            var protocol = await _context.Protocols
                .Include(p => p.Order)
                .Include(p => p.Vehicle)
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(p => p.Id == protocolId)
                ?? throw new KeyNotFoundException("Protokół nie istnieje.");

            var allVehicleDamages = await _context.Damages
                .Where(d => d.VehicleId == protocol.VehicleId && !d.IsFixed)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            var responseDto = protocol.ToResponseDto();

            responseDto.Damages = allVehicleDamages.Select(d =>
            {
                var damageDto = d.ToResponseDto();
                damageDto.IsNew = (d.ProtocolId == protocolId);
                damageDto.CreatedAt = d.CreatedAt;
                return damageDto;
            }).ToList();

            return responseDto;
        }
    }
}
