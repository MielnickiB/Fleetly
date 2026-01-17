using System.Security.Claims;
using System.Text.Json;
using FleetlyMobile.Constants;
using Fleetly.Shared.Dto.UserDtos;
using Microsoft.AspNetCore.Components.Authorization;

namespace FleetlyMobile.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await SecureStorage.Default.GetAsync(AppConstants.AuthTokenKey);
                var userJson = await SecureStorage.Default.GetAsync(AppConstants.UserDataKey);

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userJson))
                {
                    return NotAuthorized();
                }

                var user = JsonSerializer.Deserialize<UserResponseDto>(userJson);

                if (user == null) return NotAuthorized();

                var claims = new List<Claim>
                {
                    new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new (ClaimTypes.Email, user.Email),
                    new (ClaimTypes.Role, user.RoleName ?? string.Empty),
                    new ("FullName", $"{user.Details.Name} {user.Details.Surname}")
                };

                var identity = new ClaimsIdentity(claims, "jwt");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                return NotAuthorized();
            }
        }

        public void NotifyUserLogin()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void NotifyUserLogout()
        {

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private static AuthenticationState NotAuthorized()
            => new(new ClaimsPrincipal(new ClaimsIdentity()));
    }
}