using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.UserDetailsDtos
{
    public class UserDetailsUpdateDto
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        [Phone(ErrorMessage = "Nieporawny format numeru telefonu!")]
        public string? PhoneNumber { get; set; }
        public string? Company { get; set; }
    }
}
