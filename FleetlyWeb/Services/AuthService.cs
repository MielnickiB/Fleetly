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
                return "Brak odpowiedzi z serwera. Spróbuje ponownie";
            }
            if (!resp.Success)
            {
                return resp.Error ?? "Nieznany błąd podczas logowania.";
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

        public async Task<UserResponseDto?> Register(UserRegisterDto dto)
        {
            var resp = await _api.PostAsync<UserRegisterDto, UserResponseDto>("api/Auth/register", dto);
            if (resp is null || !resp.Success)
                return null;

            return resp.Data;
        }
    }
}
