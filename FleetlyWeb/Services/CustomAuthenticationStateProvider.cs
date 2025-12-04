using Fleetly.Shared.Dto.UserDtos;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace FleetlyWeb.Services
{
    public class CustomAuthStateProvider(ILocalStorage storage) : AuthenticationStateProvider
    {
        private readonly ILocalStorage _storage = storage;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _storage.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return EmptyState();

            var userJson = await _storage.GetUserJsonAsync();
            if (string.IsNullOrWhiteSpace(userJson))
                return EmptyState();

            UserResponseDto? user;
            try
            {
                user = JsonSerializer.Deserialize<UserResponseDto>(userJson);
            }
            catch (JsonException)
            {
                return EmptyState();
            }
            catch (ArgumentNullException)
            {
                return EmptyState();
            }
            if (user == null)
                return EmptyState();

            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrEmpty(user.RoleName))
                return EmptyState();


            var identity = new ClaimsIdentity(
            [
            new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.RoleName)
            ], "jwt");

            var principal = new ClaimsPrincipal(identity);

            return new AuthenticationState(principal);
        }

        private static AuthenticationState EmptyState()
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public void NotifyAuthStateChanged()
            => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
