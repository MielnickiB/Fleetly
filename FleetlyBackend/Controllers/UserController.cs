using Fleetly.Shared.Dto.UserDtos;
using FleetlyBackend.Helpers;
using FleetlyBackend.Services.UserSerivce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController(IUserService service) : ControllerBase
    {
        private readonly IUserService _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers(int page = 1, int pageSize = 20)
        {
            return Ok(await _service.GetAll(page, pageSize));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponseDto>> GetById(int id)
        {
            if(id <= 0)
            {
                return BadRequest(new { error = "Niepoprawne ID." });
            }
            var user = await _service.GetById(id);
            return user is null ? NotFound("Użytkownik nie istnieje.") : Ok(user);
        }

        [HttpGet("my")]
        public async Task<ActionResult<UserResponseDto>> GetMe()
        {
            if(!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);

            var user = await _service.GetCurrent(userId);

            return user is null ? NotFound("Użytkownik nie istnieje.") : Ok(user);
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
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpPut("my")]
        public async Task<ActionResult<UserResponseDto>> UpdateMe(UserUpdateDto dto)
        {
            if (!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);
            try {
                return Ok(await _service.Update(userId, dto));
            }
            catch (ArgumentException ex) {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponseDto>> Update(int id, UserUpdateDto dto)
        {
            if(id <= 0)
            {
                return BadRequest(new { error = "Niepoprawne ID." });
            }
            try {
                return Ok(await _service.Update(id, dto));

            } 
            catch (ArgumentException ex) {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpDelete("/my")]
        public async Task<ActionResult<bool>> DeactivateMe()
        {
            if (!User.TryGetUserId(out var userId, out var error))
                return Unauthorized(error);
            try { 
                return Ok(await _service.Deactivate(userId)); 
            }
            catch (ArgumentException ex) {
                return BadRequest(new { error = ex.Message }); 
            }
            catch (InvalidOperationException ex) {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> Deactivate(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Niepoprawne ID." });
            }
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
