using FleetlyBackend.Services.UserDetailsService;
using FleetlyBackend.Helpers;
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
            var details = await _service.Get();
            return details is null ? NotFound("Nie znaleziono szczegółów użytkownika") : Ok(details);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<UserDetailsDto>> UpdateMyDetails(UserDetailsUpdateDto dto)
        {
            return Ok(await _service.Update(dto));
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
            return Ok(await _service.Update(dto, userId));
        }
    }
}
