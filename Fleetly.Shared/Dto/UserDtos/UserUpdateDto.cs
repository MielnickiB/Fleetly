using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.UserDtos
{
    public class UserUpdateDto
    {
        [EmailAddress]
        public string? Email { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "RoleId musi być większe od 0.")]
        public int? RoleId { get; set; }
    }
}
