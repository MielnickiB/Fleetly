using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.UserDtos;
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
        public async Task<ActionResult<PagedResult<UserResponseDto>>> GetAllUsers()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("role")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllUsersByRole(string roleName)
        {
            ArgumentNullException.ThrowIfNull(roleName);
            return Ok(await _service.GetAllByRole(roleName));
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<UserResponseDto?>> GetById(int id)
        {
            try
            {
                var user = await _service.GetById(id);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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

        [HttpPut("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto dto)
        {
            try 
            {
                await _service.ChangePasswordAsync(dto);
                return Ok(true);
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
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
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
