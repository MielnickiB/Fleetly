using System.ComponentModel.DataAnnotations;
using Fleetly.Shared.Dto.UserDetailsDtos;

namespace Fleetly.Shared.Dto.UserDtos
{
    public class UserUpdateDto
    {
        [EmailAddress]
        public string? Email { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "RoleId musi być większe od 0.")]
        public int? RoleId { get; set; }
        public UserDetailsUpdateDto? DetailsUpdateDto { get; set; }
    }
}
