using Fleetly.Shared.Dto.UserDetailsDtos;
using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.AuthDtos
{
    public class UserRegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public UserDetailsDto Details { get; set; } = new UserDetailsDto();
    }
}
