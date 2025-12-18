using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.UserDtos;

namespace FleetlyWeb.Services.Users
{
    public class UserService(ApiClient api) : IUserService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/Users";

        public async Task<ApiResponse<PagedResult<UserResponseDto>>> GetAllUsersAsync()
        {
            var resp = await _api.GetAsync<PagedResult<UserResponseDto>>(BaseUrl)
                ?? throw new Exception("Brak odpowiedzi z serwera.");
            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się pobrać użytkowników.");
            }
            return resp!;
        }

        public async Task<ApiResponse<UserResponseDto?>> GetUserByIdAsync(int userId)
        {
            var resp = await _api.GetAsync<UserResponseDto?>($"{BaseUrl}/{userId}")
                ?? throw new Exception("Brak odpowiedzi z serwera.");
            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się pobrać użytkownika.");
            }
            return resp;
        }

        public async Task<ApiResponse<UserResponseDto>> CreateUserAsync(UserCreateDto dto)
        {
            var resp = await _api.PostAsync<UserCreateDto, UserResponseDto>(BaseUrl, dto)
                ?? throw new Exception("Brak odpowiedzi z serwera.");
            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się utworzyć użytkownika.");
            }
            return resp!;
        }

        public async Task<ApiResponse<UserResponseDto>> UpdateUserAsync(int id, UserUpdateDto dto)
        {
            var resp = await _api.PutAsync<UserUpdateDto, UserResponseDto>($"{BaseUrl}/{id}", dto)
                ?? throw new Exception("Brak odpowiedzi z serwera.");
            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się zaktualizować użytkownika.");
            }
            return resp!;
        }

        public async Task<ApiResponse<bool>> DeactivateUserAsync(int id)
        {
            var resp = await _api.DeleteAsync($"{BaseUrl}/{id}")
                ?? throw new Exception("Brak odpowiedzi z serwera.");
            if (!resp.Success)
            {
                throw new Exception(resp.Error ?? "Nie udało się dezaktywować użytkownika.");
            }
            return resp!;
        }
    }
}
