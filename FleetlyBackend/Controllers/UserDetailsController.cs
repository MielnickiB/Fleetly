using FleetlyBackend.Services.UserDetailsService;
using FleetlyBackend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fleetly.Shared.Dto.UserDetailsDtos;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserDetailsController(IUserDetailsService service) : ControllerBase
    {
        private readonly IUserDetailsService _service = service;

        [HttpGet("my")]
        public async Task<ActionResult<UserDetailsDto>> GetMyDetails()
        {
            if (!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);

            var details = await _service.Get(userId);

            return details is null ? NotFound() : Ok(details);
        }

        [HttpGet("{userId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDetailsDto>> GetForUser(int userId)
        {
            if( userId <= 0 )
                return BadRequest(new { error = "Nieprawidłowe ID." });
            var details = await _service.Get(userId);
            return details is null ? NotFound() : Ok(details);
        }

        [HttpPut("my")]
        public async Task<ActionResult<UserDetailsDto>> UpdateMyDetails(UserDetailsUpdateDto dto)
        {
            if (!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);

            return Ok(await _service.Update(userId, dto));
        }

        [HttpPut("{userId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDetailsDto>> UpdateDetails(int userId, UserDetailsUpdateDto dto)
        {
            if (userId <= 0)
                return BadRequest(new { error = "Nieprawidłowe ID." });
            try
            {
                var updated = await _service.Update(userId, dto);
                return Ok(updated);
            }
            catch (InvalidOperationException ex) 
            { 
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
