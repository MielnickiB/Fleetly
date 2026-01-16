using Fleetly.Shared.Client;
using Fleetly.Shared.Dto.NotificationDtos;

namespace FleetlyMobile.Services.Notifications
{
    public class NotificationService(ApiClient api) : INotificationService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/Notification";

        public event Action? OnChange;

        public async Task<ApiResponse<List<NotificationResponseDto>?>> GetMyNotificationsAsync()
        {
            return await _api.GetAsync<List<NotificationResponseDto>>(BaseUrl);
        }

        public async Task<ApiResponse<int>> GetUnreadCountAsync()
        {
            return await _api.GetAsync<int>($"{BaseUrl}/unread-count");
        }

        public async Task<ApiResponse<bool>> MarkAsReadAsync(int id)
        {
            var res = await _api.PutAsync<object, bool>($"{BaseUrl}/{id}/read", null!);
            if (res.Success) NotifyStateChanged();
            return res;
        }

        public async Task<ApiResponse<bool>> MarkAllAsReadAsync()
        {
            var res = await _api.PutAsync<object, bool>($"{BaseUrl}/read-all", null!);
            if (res.Success) NotifyStateChanged();
            return res;
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var res = await _api.DeleteAsync($"{BaseUrl}/{id}");
            if (res.Success) NotifyStateChanged();
            return res;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
