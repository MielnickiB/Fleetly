using Fleetly.Shared.Dto.UserDetailsDtos;
using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.UserDtos
{
    public class UserBaseDto
    {
        [Required(ErrorMessage = "Email jest wymagany!")]
        [EmailAddress(ErrorMessage = "Błędny format email!")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rola jest wymagana!")]
        public int RoleId { get; set; }

        public UserDetailsDto Details { get; set; } = new();
    }
}
