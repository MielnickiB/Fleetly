using Fleetly.Shared.Dto.UserDetailsDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Mappings
{
    public static class UserDetailsMappings
    {
        public static UserDetailsDto ToDto(this UserDetails details)
            => new()
            {
                Name = details.Name,
                Surname = details.Surname,
                PhoneNumber = details.PhoneNumber,
                Company = details.Company
            };
    }
}
