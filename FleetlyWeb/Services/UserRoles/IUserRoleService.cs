using Fleetly.Shared.Dto.UserRoleDtos;
using Fleetly.Shared.Client;

namespace FleetlyWeb.Services.UserRoles
{
    public interface IUserRoleService
    {
        public Task<ApiResponse<List<UserRoleResponseDto>>> GetAllRolesAsync();
    }
}
