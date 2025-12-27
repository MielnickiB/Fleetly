using Fleetly.Shared.Dto.NotificationDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Services.NotificationService
{
    public interface INotificationService
    {
        Task<List<NotificationResponseDto>> GetMyNotifications();
        Task<int> GetUnreadCount();
        Task MarkAsRead(int id);
        Task MarkAllAsRead();
        Task Delete(int id);

        Task NotifyOrderCreated(Order order);
        Task NotifyOrderUpdated(Order order, int? modifiedByUserId);
        Task NotifyOrderCancelled(Order order, int? cancelledByUserId);

        Task NotifyWorkerAssigned(Order order, int workerId);
        Task NotifyWorkerUnassigned(Order order, int workerId);
        Task NotifyWorkerAccepted(Order order);
        Task NotifyWorkerResigned(Order order);

        Task NotifyOrderFinishedByWorker(Order order);
        Task NotifyOrderApprovedByAdmin(Order order);
    }
}