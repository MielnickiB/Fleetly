using System.ComponentModel.DataAnnotations;
using Fleetly.Shared.Dto.UserDetailsDtos;

namespace Fleetly.Shared.Dto.UserDtos
{
    public class UserUpdateDto
    {
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu email!")]
        public string? Email { get; set; }
        public string? Password { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Niepoprawna rola użytkownika!")]
        public int? RoleId { get; set; }
        public UserDetailsUpdateDto? Details { get; set; }
    }
}
