using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.UserDetailsDtos
{
    public class UserDetailsDto
    {
        [Required(ErrorMessage = "Imię użytkownika jest wymagane!")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Nazwisko użytkownika jest wymagane!")]
        public string Surname { get; set; } = null!;

        [Required(ErrorMessage = "Numer telefonu użytkownika jest wymagany!")]
        [Phone(ErrorMessage = "Nieporawny format numeru telefonu!")]
        public string PhoneNumber { get; set; } = null!;
        public string? Company { get; set; }
    }
}