using Fleetly.Shared.Dto.UserRoleDtos;

namespace FleetlyWeb.Services.UserRoles
{
    public interface IUserRoleService
    {
        public Task<ApiResponse<List<UserRoleResponseDto>>> GetAllRolesAsync();
    }
}
