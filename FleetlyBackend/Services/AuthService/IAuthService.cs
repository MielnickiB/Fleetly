using Fleetly.Shared.Dto.AuthDtos;
using Fleetly.Shared.Dto.UserDtos;

namespace FleetlyBackend.Services.AuthService
{
    public interface IAuthService
    {
        Task<UserResponseDto?> RegisterAsync(UserRegisterDto request);
        Task<AuthResultDto?> LoginAsync(UserLoginDto request);
    }
}
