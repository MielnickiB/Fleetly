using Fleetly.Shared.Dto.UserDtos;

namespace FleetlyWeb.Services
{
    public interface ILocalStorage
    {
        Task SaveLoginAsync(string token, UserResponseDto user);
        Task<string?> GetTokenAsync();
        Task<string?> GetUserJsonAsync();
        Task LogoutAsync();
    }
}
