using Fleetly.Shared.Client;
using Fleetly.Shared.Dto.AuthDtos;
using FleetlyMobile.Constants;
using System.Text.Json;

namespace FleetlyMobile.Services.Auth
{
    public class AuthService(ApiClient api, CustomAuthStateProvider authStateProvider) : IAuthService
    {
        private readonly ApiClient _api = api;
        private readonly CustomAuthStateProvider _authStateProvider = authStateProvider;
        private const string _loginEndpoint = "api/Auth/login";
        private bool _isLogoutInProgress = false;

        public async Task<string?> LoginAsync(UserLoginDto dto)
        {
            var response = await _api.PostAsync<UserLoginDto, AuthResultDto>(_loginEndpoint, dto);

            if (!response.Success)
            {
                return response.Error ?? "Nie udało się zalogować.";
            }

            var result = response.Data;
            if (result != null && !string.IsNullOrEmpty(result.AccessToken) && result.User != null)
            {
                await SecureStorage.Default.SetAsync(AppConstants.AuthTokenKey, result.AccessToken);

                var userJson = JsonSerializer.Serialize(result.User);
                await SecureStorage.Default.SetAsync(AppConstants.UserDataKey, userJson);

                _authStateProvider.NotifyUserLogin();
                return null;
            }

            return "Otrzymano nieprawidłowe dane z serwera.";
        }

        public Task LogoutAsync()
        {
            if (_isLogoutInProgress)
                return Task.CompletedTask;

            try
            {
                _isLogoutInProgress = true;

                SecureStorage.Default.Remove(AppConstants.AuthTokenKey);
                SecureStorage.Default.Remove(AppConstants.UserDataKey);

                _authStateProvider.NotifyUserLogout();
            }
            finally
            {
                _isLogoutInProgress = false;
            }

            return Task.CompletedTask;
        }
    }
}