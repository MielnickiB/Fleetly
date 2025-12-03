using Fleetly.Shared.Dto.UserRoleDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Mappings
{
    public static class UserRoleMappings
    {
        public static UserRoleResponseDto ToResponseDto(this UserRole r)
            => new()
            {
                Id = r.Id,
                RoleName = r.RoleName
            };
    }
}
