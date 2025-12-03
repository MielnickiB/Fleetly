using Fleetly.Shared.Dto.UserDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Mappings
{
    public static class UserMappings
    {
        public static UserResponseDto ToResponseDto(this User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role.RoleName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };
        }
    }
}
