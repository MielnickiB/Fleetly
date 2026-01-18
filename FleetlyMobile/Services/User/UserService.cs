using Fleetly.Shared.Client;
using Fleetly.Shared.Dto.UserDtos;

namespace FleetlyMobile.Services.User
{
    public class UserService(ApiClient api) : IUserService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/Users";
        public async Task<ApiResponse<UserResponseDto?>> GetByIdAsync(int id)
        {
            return await _api.GetAsync<UserResponseDto>($"{BaseUrl}/{id}");
        }
        public async Task<ApiResponse<UserResponseDto?>> UpdateUserAsync(int id, UserUpdateDto dto)
        {
            return await _api.PutAsync<UserUpdateDto, UserResponseDto>($"{BaseUrl}/{id}", dto);
        }
        public async Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto dto)
        {
            return await _api.PutAsync<ChangePasswordDto, bool>($"{BaseUrl}/change-password", dto);
        }
    }
}
