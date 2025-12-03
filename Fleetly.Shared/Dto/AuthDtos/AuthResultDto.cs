using Fleetly.Shared.Dto.UserDtos;

namespace Fleetly.Shared.Dto.AuthDtos
{
    public class AuthResultDto
    {
        public required string AccessToken { get; set; }
        public UserResponseDto? User { get; set; }
    }
}