using Fleetly.Shared.Client;
using Fleetly.Shared.Dto.UserDtos;

namespace FleetlyMobile.Services.User
{
    public interface IUserService
    {
        public Task<ApiResponse<UserResponseDto?>> GetByIdAsync(int id);
        public Task<ApiResponse<UserResponseDto?>> UpdateUserAsync(int id, UserUpdateDto dto);
    }
}
