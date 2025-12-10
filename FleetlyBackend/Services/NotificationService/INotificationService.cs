using Fleetly.Shared.Dto.NotificationDtos;

namespace FleetlyBackend.Services.NotificationService
{
    public interface INotificationService
    {
        Task<List<NotificationResponseDto>> GetAll();
        Task<NotificationResponseDto> Create(NotificationCreateDto dto);
        Task<bool> MarkAsRead(int id);
        Task<bool> Delete(int id);
    }
}
