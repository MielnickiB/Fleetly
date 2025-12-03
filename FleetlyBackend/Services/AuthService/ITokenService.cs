using FleetlyBackend.Models;

namespace FleetlyBackend.Services.AuthService
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
    }
}
