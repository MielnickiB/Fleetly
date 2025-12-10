using FleetlyBackend.Services.UserDetailsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fleetly.Shared.Dto.UserDetailsDtos;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailsController(IUserDetailsService service) : ControllerBase
    {
        private readonly IUserDetailsService _service = service;

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserDetailsDto>> GetMyDetails()
        {
            try
            {
                var details = await _service.Get();
                return details is null ? NotFound("Nie znaleziono szczegółów użytkownika") : Ok(details);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<UserDetailsDto>> UpdateMyDetails(UserDetailsUpdateDto dto)
        {
            try
            {
                return Ok(await _service.Update(dto));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpGet("{userId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDetailsDto>> GetUserDetails(int userId)
        {
            var details = await _service.Get(userId);
            return details is null ? NotFound("Nie znaleziono szczegółów użytkownika") : Ok(details);
        }

        [HttpPut("{userId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDetailsDto>> UpdateUserDetails(int userId, UserDetailsUpdateDto dto)
        {
            try {                 
                return Ok(await _service.Update(dto, userId));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
