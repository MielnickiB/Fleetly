using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.AuthDtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Adres email jest wymagany")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu email")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
