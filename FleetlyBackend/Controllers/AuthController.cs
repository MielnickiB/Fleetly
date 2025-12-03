using Fleetly.Shared.Dto.AuthDtos;
using Fleetly.Shared.Dto.UserDtos;
using FleetlyBackend.Services.AuthService;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(UserRegisterDto request)
        {
            try
            {
                var user = await authService.RegisterAsync(request);
                return user is null ? BadRequest("Rejestracja nie powiodła się.") : Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResultDto>> Login(UserLoginDto request)
        {
            try
            {
                var result = await authService.LoginAsync(request);
                return result is null ? NotFound("Użytkownik nie znaleziony.") : Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
}
