using Fleetly.Shared.Dto.UserRoleDtos;

namespace FleetlyBackend.Services.UserRoleService
{
    public interface IUserRoleService
    {
        Task<List<UserRoleResponseDto>> GetAll(int page = 1, int pageSize = 10);
        Task<UserRoleResponseDto?> Get(int id);
        Task<UserRoleResponseDto> Create(UserRoleCreateDto dto);
        Task<UserRoleResponseDto> Update(int id, UserRoleUpdateDto dto);
        Task<bool> Delete(int id);
    }
}
