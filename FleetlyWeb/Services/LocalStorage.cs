using Fleetly.Shared.Dto.UserDtos;
using Microsoft.JSInterop;
using System.Text.Json;

namespace FleetlyWeb.Services
{
    public class LocalStorage(IJSRuntime js) : ILocalStorage
    {
        private readonly IJSRuntime _js = js;

        private const string TokenKey = "fleetly_token";
        private const string UserKey = "fleetly_user";

        public async Task SaveLoginAsync(string token, UserResponseDto user)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
            await _js.InvokeVoidAsync("localStorage.setItem", UserKey, JsonSerializer.Serialize(user));
        }

        public async Task<string?> GetTokenAsync()
            => await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);

        public async Task<string?> GetUserJsonAsync()
            => await _js.InvokeAsync<string?>("localStorage.getItem", UserKey);

        public async Task LogoutAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            await _js.InvokeVoidAsync("localStorage.removeItem", UserKey);
        }
    }
}
