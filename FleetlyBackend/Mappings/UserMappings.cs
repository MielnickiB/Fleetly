using Fleetly.Shared.Dto.UserDtos;
using Fleetly.Shared.Dto.UserDetailsDtos;
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
                Details = new UserDetailsDto
                {
                    Name = user.Details.Name,
                    Surname = user.Details.Surname,
                    PhoneNumber = user.Details.PhoneNumber,
                    Company = user.Details?.Company,
                },
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };
        }
    }
}
