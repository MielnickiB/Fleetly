using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.UserDetailsDtos
{
    public class UserDetailsUpdateDto
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public string? Company { get; set; }
    }
}
