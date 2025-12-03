using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.UserRoleDtos
{
    public class UserRoleUpdateDto
    {
        [Required]
        [StringLength(30)]
        public string RoleName { get; set; } = null!;
    }
}
