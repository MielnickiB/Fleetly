using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.UserDtos
{
    public class UserCreateDto : UserBaseDto
    {
        [Required(ErrorMessage = "Hasło jest wymagane")]
        [MinLength(3, ErrorMessage = "Hasło musi mieć min. 3 znaki")]
        public string Password { get; set; } = string.Empty;
    }
}
