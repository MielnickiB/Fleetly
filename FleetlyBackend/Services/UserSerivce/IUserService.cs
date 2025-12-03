using Fleetly.Shared.Dto.UserDtos;

namespace FleetlyBackend.Services.UserSerivce
{
    public interface IUserService
    {
        public Task<List<UserResponseDto>> GetAll(int page = 1, int pageSize = 10);
        public Task<List<UserResponseDto>> GetAllByRole(string roleName, int page = 1, int pageSize = 10);
        public Task<UserResponseDto?> GetById(int id);
        public Task<UserResponseDto?> GetCurrent(int userId);
        public Task<UserResponseDto?> Create(UserCreateDto newUser);
        public Task<UserResponseDto> Update(int id, UserUpdateDto updatedUser);
        public Task<bool> Deactivate(int id);
    }
}
