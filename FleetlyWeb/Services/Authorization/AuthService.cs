using Fleetly.Shared.Dto.AuthDtos;
using Fleetly.Shared.Dto.UserDtos;
using Blazored.LocalStorage;

namespace FleetlyWeb.Services.Authorization
{
    public class AuthService(ApiClient api, ILocalStorageService localStorage, CustomAuthStateProvider state) : IAuthService
    {
        private readonly ApiClient _api = api;
        private readonly ILocalStorageService _localStorage = localStorage;
        private readonly CustomAuthStateProvider _state = state;
        private readonly string _baseUrl = "api/Auth/";
        public async Task<string?> Login(UserLoginDto dto)
        {
            var resp = await _api.PostAsync<UserLoginDto, AuthResultDto>($"{_baseUrl}login", dto);

            if (resp is null)
            {
                return "Brak odpowiedzi z serwera. Spróbuj ponownie";
            }
            if (!resp.Success)
            {
                return resp.Error ?? "Nie udało się zalogować. Spróbuje ponownie";
            }

            var authResult = resp.Data;
            if (authResult is not null && !string.IsNullOrEmpty(authResult.AccessToken) && authResult.User is not null)
            {
                if(!authResult.User.IsActive)
                    return "Konto jest nieaktywne. Skontaktuj się z administratorem.";

                await _localStorage.SetItemAsync("fleetly_token", authResult.AccessToken);
                await _localStorage.SetItemAsync("fleetly_user", authResult.User);

                _state.NotifyAuthStateChanged();
                return null;
            }

            return resp.Error;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("fleetly_token");
            await _localStorage.RemoveItemAsync("fleetly_user");
            _state.NotifyAuthStateChanged();
        }

        public async Task<string?> Register(UserRegisterDto dto)
        {
            var resp = await _api.PostAsync<UserRegisterDto, UserResponseDto>($"{_baseUrl}register", dto);
            if (resp is null)
            {
                return "Brak odpowiedzi z serwera. Spróbuj ponownie";
            }
            if (!resp.Success)
            {
                return resp.Error ?? "Nie udało się zarejestrować. Spróbuj ponownie";
            }

            var authResult = resp.Data;
            if (authResult is not null)
            {
                return null;
            }

            return resp.Error;
        }
    }
}
