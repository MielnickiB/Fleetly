using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.UserDtos
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Obecne hasło jest wymagane!")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nowe hasło jest wymagane!")]
        [MinLength(6, ErrorMessage = "Hasło musi mieć minimum 6 znaków!")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane!")]
        [Compare(nameof(NewPassword), ErrorMessage = "Hasła muszą być identyczne!")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
