using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.UserDetailsDtos
{
    public class UserDetailsDto
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Surname { get; set; } = null!;
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = null!;
        public string? Company { get; set; }
    }
}