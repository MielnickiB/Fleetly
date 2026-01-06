using Fleetly.Shared.Dto.NotificationDtos;
using Fleetly.Shared.Client;

namespace FleetlyWeb.Services.Notifications
{
    public interface INotificationService
    {
        Task<ApiResponse<List<NotificationResponseDto>?>> GetMyNotificationsAsync();
        Task<ApiResponse<int>> GetUnreadCountAsync();
        Task<ApiResponse<bool>> MarkAsReadAsync(int id);
        Task<ApiResponse<bool>> MarkAllAsReadAsync();
        Task<ApiResponse<bool>> DeleteAsync(int id);

        event Action? OnChange;
    }
}
