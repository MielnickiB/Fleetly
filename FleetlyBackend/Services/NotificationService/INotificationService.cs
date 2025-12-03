using Fleetly.Shared.Dto.NotificationDtos;

namespace FleetlyBackend.Services.NotificationService
{
    public interface INotificationService
    {
        Task<List<NotificationResponseDto>> GetForUser(int userId);
        Task<NotificationResponseDto?> Get(int id, int userId);
        Task<NotificationResponseDto> Create(NotificationCreateDto dto);
        Task<bool> MarkAsRead(int id, int userId);
        Task<bool> Delete(int id, int userId);
    }
}
