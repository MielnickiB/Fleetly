using Fleetly.Shared.Dto.AuthDtos;
using Fleetly.Shared.Dto.UserDtos;

namespace FleetlyWeb.Services
{
    public interface IAuthService
    {
        public Task<bool?> Login(UserLoginDto dto);
        public Task Logout();
        public Task<UserResponseDto?> Register(UserRegisterDto dto);
    }
}
