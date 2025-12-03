using Fleetly.Shared.Dto.UserDetailsDtos;
using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.UserDtos
{
    public class UserCreateDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Id roli musi mieć pozytywną wartość")]
        public int RoleId { get; set; }
        [Required]
        public UserDetailsDto Details { get; set; } = new UserDetailsDto();
    }
}
