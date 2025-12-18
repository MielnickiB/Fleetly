using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.UserDtos;

namespace FleetlyWeb.Services.Users
{
    public interface IUserService
    {
        Task<ApiResponse<PagedResult<UserResponseDto>>> GetAllUsersAsync();
        Task<ApiResponse<UserResponseDto?>> GetUserByIdAsync(int userId);
        Task<ApiResponse<UserResponseDto>> CreateUserAsync(UserCreateDto dto);
        Task<ApiResponse<UserResponseDto>> UpdateUserAsync(int id, UserUpdateDto dto);
        Task<ApiResponse<bool>> DeactivateUserAsync(int id);
    }
}
