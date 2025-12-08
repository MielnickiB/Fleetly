using Fleetly.Shared.Dto.AuthDtos;

namespace FleetlyWeb.Services
{
    public interface IAuthService
    {
        public Task<string?> Login(UserLoginDto dto);
        public Task Logout();
        public Task<string?> Register(UserRegisterDto dto);
    }
}
