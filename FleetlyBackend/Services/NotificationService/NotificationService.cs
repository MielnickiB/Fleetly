using Fleetly.Shared.Dto.NotificationDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.NotificationService
{
    public class NotificationService(FleetlyContext context) : INotificationService
    {
        private readonly FleetlyContext _context = context;

        public async Task<List<NotificationResponseDto>> GetForUser(int userId)
            => await _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => n.ToResponseDto())
                .ToListAsync();

        public async Task<NotificationResponseDto?> Get(int id, int userId)
        {
            var notif = await _context.Notifications
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            return notif?.ToResponseDto();
        }

        public async Task<NotificationResponseDto> Create(NotificationCreateDto dto)
        {
            var exists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!exists)
                throw new ArgumentException("Użytkownik nie istnieje.");

            var n = new Notification
            {
                UserId = dto.UserId,
                Type = dto.Type,
                Title = dto.Title,
                Message = dto.Message,
                RelatedEntityId = dto.RelatedEntityId,
                RelatedEntity = dto.RelatedEntity
            };

            _context.Notifications.Add(n);
            await _context.SaveChangesAsync();

            return n.ToResponseDto();
        }

        public async Task<bool> MarkAsRead(int id, int userId)
        {
            var notif = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId)
                ?? throw new ArgumentException("Notyfikacja nie istnieje.");

            notif.IsRead = true;
            notif.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id, int userId)
        {
            var notif = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId) 
                ?? throw new ArgumentException("Notyfikacja nie istnieje.");

            _context.Notifications.Remove(notif);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
