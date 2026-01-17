using Fleetly.Shared.Dto.UserDtos;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Blazored.LocalStorage;
using FleetlyWeb.Constants;

namespace FleetlyWeb.Services
{
    public class CustomAuthStateProvider(ILocalStorageService localStorage) : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage = localStorage;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>(StorageKeys.AccessToken);
                var user = await _localStorage.GetItemAsync<UserResponseDto>(StorageKeys.UserProfile);

                if (string.IsNullOrWhiteSpace(token) || user == null)
                    return EmptyState();

                if (!user.IsActive)
                    return EmptyState();

                var identity = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleName ?? string.Empty),
                    new Claim("FullName", $"{user.Details.Name} {user.Details.Surname}"),
                ], "jwt");

                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Bład pobierania danych autoryzacji: {ex.Message}");
                return EmptyState();
            }
        }

        private static AuthenticationState EmptyState()
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public void NotifyAuthStateChanged()
            => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
