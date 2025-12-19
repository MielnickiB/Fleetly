using Fleetly.Shared.Dto.UserDetailsDtos;
using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.UserDtos
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Email jest wymagany!")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Hasło jest wymagane!")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Rola jest wymagana!")]
        [Range(1, int.MaxValue, ErrorMessage = "Niepoprawna rola użytkownika!")]
        public int RoleId { get; set; }
        [Required]
        public UserDetailsDto Details { get; set; } = new UserDetailsDto();
    }
}
