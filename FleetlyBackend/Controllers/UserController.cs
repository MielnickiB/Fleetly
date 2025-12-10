using Fleetly.Shared.Dto.UserDtos;
using FleetlyBackend.Helpers;
using FleetlyBackend.Services.UserSerivce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService service) : ControllerBase
    {
        private readonly IUserService _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _service.GetAll(page, pageSize));
        }

        [HttpGet("role")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllUsersByRole(string roleName)
        {
            ArgumentNullException.ThrowIfNull(roleName);
            return Ok(await _service.GetAllByRole(roleName));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponseDto>> GetById(int id)
        {
            try
            {
                var user = await _service.GetById(id);
                return user is null ? NotFound("Użytkownik nie istnieje.") : Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserResponseDto>> Get()
        {
            try
            {
                var user = await _service.Get();
                return user is null ? NotFound("Użytkownik nie istnieje.") : Ok(user);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponseDto>> Create(UserCreateDto dto)
        {
            try { 
                var createdUser = await _service.Create(dto);
                return createdUser is null ? BadRequest("Nie udało się utworzyć użytkownika.") : Ok(createdUser);
            } 
            catch (ArgumentException ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<UserResponseDto>> Update(int id, UserUpdateDto dto)
        {
            try {
                return Ok(await _service.Update(id, dto));

            } 
            catch (ArgumentException ex) {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> Deactivate(int id)
        {
            try {
                return Ok(await _service.Deactivate(id));
            }
            catch (ArgumentException ex) {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
