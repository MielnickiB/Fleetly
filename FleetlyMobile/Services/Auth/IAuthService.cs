using Fleetly.Shared.Dto.AuthDtos;

namespace FleetlyMobile.Services.Auth
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(UserLoginDto dto);
        Task LogoutAsync();
    }
}
