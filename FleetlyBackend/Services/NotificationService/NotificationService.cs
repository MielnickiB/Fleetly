using Fleetly.Shared.Dto.NotificationDtos;
using Fleetly.Shared.Enums;
using FleetlyBackend.Data;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using FleetlyBackend.Extensions;
using FleetlyBackend.Services.UserSerivce;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.NotificationService
{
    public class NotificationService(
        FleetlyContext context,
        IHttpContextAccessor http,
        IUserService userService
        ) : INotificationService
    {
        private readonly FleetlyContext _context = context;
        private readonly IHttpContextAccessor _http = http;
        private readonly IUserService _userService = userService;

        public async Task<List<NotificationResponseDto>> GetMyNotifications()
        {
            var userId = _http.CurrentUser().GetUserId();

            return await _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(100)
                .Select(n => n.ToResponseDto())
                .ToListAsync();
        }

        public async Task<int> GetUnreadCount()
        {
            var userId = _http.CurrentUser().GetUserId();
            return await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAsRead(int id)
        {
            var userId = _http.CurrentUser().GetUserId();
            var notif = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (notif != null && !notif.IsRead)
            {
                notif.IsRead = true;
                notif.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsRead()
        {
            var userId = _http.CurrentUser().GetUserId();
            var unread = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in unread)
            {
                n.IsRead = true;
                n.ReadAt = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var userId = _http.CurrentUser().GetUserId();
            await _context.Notifications
                .Where(n => n.Id == id && n.UserId == userId)
                .ExecuteDeleteAsync();
        }

        public async Task NotifyOrderCreated(Order order)
        {
            var admins = await _userService.GetAllByRole("Admin");

            foreach (var admin in admins)
            {
                await CreateInternal(
                    userId: admin.Id,
                    type: NotificationType.OrderCreated,
                    title: $"Nowe zlecenie #{order.Id}",
                    message: $"Klient utworzył nowe zlecenie na trasie {order.StartLocation?.City} -> {order.EndLocation?.City}.",
                    relatedId: order.Id
                );
            }
        }

        public async Task NotifyOrderUpdated(Order order, int? modifiedByUserId)
        {
            if (modifiedByUserId.HasValue && modifiedByUserId != order.ClientId)
            {
                await CreateInternal(
                    order.ClientId,
                    NotificationType.OrderUpdated,
                    $"Aktualizacja zlecenia #{order.Id}",
                    "Administrator zaktualizował szczegóły Twojego zlecenia.",
                    order.Id
                );
            }

            if (modifiedByUserId.HasValue && modifiedByUserId == order.ClientId)
            {
                var admins = await _userService.GetAllByRole("Admin");
                foreach (var admin in admins)
                {
                    await CreateInternal(admin.Id, NotificationType.OrderUpdated,
                        $"Edycja zlecenia #{order.Id}", "Klient wprowadził zmiany w zleceniu.", order.Id);
                }
            }

            if (order.WorkerId.HasValue && order.WorkerId != modifiedByUserId)
            {
                await CreateInternal(order.WorkerId.Value, NotificationType.OrderUpdated,
                       $"Zmiana w zleceniu #{order.Id}", "Dane zlecenia zostały zaktualizowane.", order.Id);
            }
        }

        public async Task NotifyWorkerAssigned(Order order, int workerId)
        {
            await CreateInternal(
                workerId,
                NotificationType.AssignedToOrder,
                "Nowe przypisanie",
                $"Zostałeś przypisany do realizacji zlecenia #{order.Id}.",
                order.Id
            );
        }

        public async Task NotifyWorkerUnassigned(Order order, int workerId)
        {
            await CreateInternal(
                workerId,
                NotificationType.UnseatedFromOrder,
                "Usunięcie ze zlecenia",
                $"Zostałeś odsunięty od zlecenia #{order.Id}.",
                order.Id
            );
        }

        public async Task NotifyWorkerAccepted(Order order)
        {
            await CreateInternal(
                order.ClientId,
                NotificationType.WorkerTookTheOrder,
                "Zlecenie przyjęte",
                $"Pracownik przyjął Twoje zlecenie #{order.Id} i przygotowuje się do realizacji.",
                order.Id
            );

            var admins = await _userService.GetAllByRole("Admin");
            foreach (var admin in admins)
            {
                await CreateInternal(admin.Id, NotificationType.WorkerTookTheOrder,
                    $"Zlecenie #{order.Id} przyjęte", $"Pracownik {order.WorkerId} rozpoczął proces.", order.Id);
            }
        }

        public async Task NotifyWorkerResigned(Order order)
        {
            var admins = await _userService.GetAllByRole("Admin");
            foreach (var admin in admins)
            {
                await CreateInternal(admin.Id, NotificationType.WorkerResignedFromOrder,
                    $"Rezygnacja ze zlecenia #{order.Id}", "Pracownik zrezygnował z realizacji.", order.Id);
            }
        }

        public async Task NotifyOrderCancelled(Order order, int? cancelledByUserId)
        {
            if (cancelledByUserId != order.ClientId)
            {
                await CreateInternal(order.ClientId, NotificationType.OrderCancelled,
                    $"Anulowano zlecenie #{order.Id}", "Twoje zlecenie zostało anulowane.", order.Id);
            }

            if (order.WorkerId.HasValue)
            {
                await CreateInternal(order.WorkerId.Value, NotificationType.OrderCancelled,
                   $"Anulowano zlecenie #{order.Id}", "Zlecenie do którego byłeś przypisany zostało anulowane.", order.Id);
            }
        }

        public async Task NotifyOrderFinishedByWorker(Order order)
        {
            var admins = await _userService.GetAllByRole("Admin");
            foreach (var admin in admins)
            {
                await CreateInternal(admin.Id, NotificationType.OrderCompleted,
                    $"Zlecenie #{order.Id} zakończone", "Pracownik zgłosił zakończenie i koszty. Wymagana akceptacja.", order.Id);
            }
        }

        public async Task NotifyOrderApprovedByAdmin(Order order)
        {
            await CreateInternal(order.ClientId, NotificationType.OrderApprovedByAdmin,
                $"Zlecenie #{order.Id} zakończone", "Administrator zatwierdził realizację. Faktura została wygenerowana.", order.Id);
        }

        private async Task CreateInternal(int userId, NotificationType type, string title, string message, int relatedId)
        {
            var notif = new Notification
            {
                UserId = userId,
                Type = type,
                Title = title,
                Message = message,
                RelatedEntity = RelatedEntityType.Order,
                RelatedEntityId = relatedId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notif);
            await _context.SaveChangesAsync();
        }
    }
}