using Fleetly.Shared.Dto.UserDetailsDtos;
using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.AuthDtos
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Adres email jest wymagany!")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu email!")]

        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Hasło jest wymagane!")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane!")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hasła nie są zgodne!")]
        public string PasswordConfirm { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sczegółowe dane użytkownika są wymagane!")]
        public UserDetailsDto Details { get; set; } = new UserDetailsDto();
    }
}
