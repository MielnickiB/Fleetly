using Fleetly.Shared.Dto.NotificationDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;
using FleetlyBackend.Extensions;

namespace FleetlyBackend.Services.NotificationService
{
    public class NotificationService(FleetlyContext context, IHttpContextAccessor http) : INotificationService
    {
        private readonly FleetlyContext _context = context;
        private readonly IHttpContextAccessor _http = http;

        public async Task<List<NotificationResponseDto>> GetAll()
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == _http.CurrentUser().GetUserId())
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => n.ToResponseDto())
                .ToListAsync();
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

        public async Task<bool> MarkAsRead(int id)
        {
            var user = _http.CurrentUser();

            var notif = await _context.Notifications.FindAsync(id)
                ?? throw new ArgumentException("Notyfikacja nie istnieje.");

            if (notif.UserId != user.GetUserId())
                throw new UnauthorizedAccessException("Nie masz uprawnień do modyfikowania tej notyfikacji.");

            notif.IsRead = true;
            notif.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var user = _http.CurrentUser();

            var notif = await _context.Notifications.FindAsync(id) 
                ?? throw new ArgumentException("Notyfikacja nie istnieje.");

            if (notif.UserId != user.GetUserId())
                throw new UnauthorizedAccessException("Nie masz uprawnień do usunięcia tej notyfikacji.");

            _context.Notifications.Remove(notif);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
