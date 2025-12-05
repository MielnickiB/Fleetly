using Fleetly.Shared.Dto.AuthDtos;
using Fleetly.Shared.Dto.UserDtos;

namespace FleetlyWeb.Services
{
    public class AuthService(ApiClient api, ILocalStorage storage, CustomAuthStateProvider state) : IAuthService
    {
        private readonly ApiClient _api = api;
        private readonly ILocalStorage _storage = storage;
        private readonly CustomAuthStateProvider _state = state;
        public async Task<string?> Login(UserLoginDto dto)
        {
            var resp = await _api.PostAsync<UserLoginDto, AuthResultDto>("api/Auth/login", dto);

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
                await _storage.SaveLoginAsync(authResult.AccessToken, authResult.User);
                _state.NotifyAuthStateChanged();
                return null;
            }

            return resp.Error;
        }

        public async Task Logout()
        {
            await _storage.LogoutAsync();
            _state.NotifyAuthStateChanged();
        }

        public async Task<string?> Register(UserRegisterDto dto)
        {
            var resp = await _api.PostAsync<UserRegisterDto, UserResponseDto>("api/Auth/register", dto);
            if (resp is null)
            {
                return "Brak odpowiedzi z serwera. Spróbuj ponownie";
            }
            if (!resp.Success)
            {
                return resp.Error ?? "Nie udało się zarejestrować. Spróbuje ponownie";
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
