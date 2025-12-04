using Fleetly.Shared.Dto.AuthDtos;
using Fleetly.Shared.Dto.UserDtos;
using Microsoft.AspNetCore.Components;

namespace FleetlyWeb.Services
{
    public class AuthService(ApiClient api, ILocalStorage storage, CustomAuthStateProvider state, NavigationManager nav) : IAuthService
    {
        private readonly ApiClient _api = api;
        private readonly ILocalStorage _storage = storage;
        private readonly CustomAuthStateProvider _state = state;
        private readonly NavigationManager _nav = nav;

        public async Task<bool?> Login(UserLoginDto dto)
        {
            var authResult = await _api.PostAsync<UserLoginDto, AuthResultDto>("api/Auth/login", dto);

            if (authResult is not null && !string.IsNullOrEmpty(authResult.AccessToken) && authResult.User is not null)
            {
                await _storage.SaveLoginAsync(authResult.AccessToken, authResult.User);
                _state.NotifyAuthStateChanged();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task Logout()
        {
            await _storage.LogoutAsync();
            _state.NotifyAuthStateChanged();
            _nav.NavigateTo("/login");
        }

        public async Task<UserResponseDto?> Register(UserRegisterDto dto)
            => await _api.PostAsync<UserRegisterDto, UserResponseDto>("api/Auth/register", dto);
    }
}
