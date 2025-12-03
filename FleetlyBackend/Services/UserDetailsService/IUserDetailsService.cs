using Fleetly.Shared.Dto.UserDetailsDtos;

namespace FleetlyBackend.Services.UserDetailsService
{
    public interface IUserDetailsService
    {
        Task<UserDetailsDto?> Get(int userId);
        Task<UserDetailsDto> Update(int userId, UserDetailsUpdateDto dto);
    }
}
