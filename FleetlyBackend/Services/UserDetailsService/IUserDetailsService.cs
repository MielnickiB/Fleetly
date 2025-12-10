using Fleetly.Shared.Dto.UserDetailsDtos;

namespace FleetlyBackend.Services.UserDetailsService
{
    public interface IUserDetailsService
    {
        Task<UserDetailsDto?> Get(int? userId = null);
        Task<UserDetailsDto> Update(UserDetailsUpdateDto dto, int? userId = null);
    }
}
