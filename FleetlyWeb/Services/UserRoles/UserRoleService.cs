using Fleetly.Shared.Dto.UserRoleDtos;
using Fleetly.Shared.Client;

namespace FleetlyWeb.Services.UserRoles
{
    public class UserRoleService(ApiClient api) : IUserRoleService
    {
        private readonly ApiClient _api = api;
        private const string BaseUrl = "api/UserRole";

        public async Task<ApiResponse<List<UserRoleResponseDto>>> GetAllRolesAsync()
        {
            var response = await _api.GetAsync<List<UserRoleResponseDto>>(BaseUrl) 
                ?? throw new Exception("Brak odpowiedzi z serwera.");

            if (!response.Success)
            {
                throw new Exception(response.Error ?? "Nie udało się pobrać ról użytkowników.");
            }

            return response!;
        }
    }
}
