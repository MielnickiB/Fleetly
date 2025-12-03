using Fleetly.Shared.Dto.UserRoleDtos;
using FleetlyBackend.Services.UserRoleService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetlyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserRoleController(IUserRoleService service) : ControllerBase
    {
        private readonly IUserRoleService _service = service;

        [HttpGet]
        public async Task<ActionResult<List<UserRoleResponseDto>>> GetAll(int page = 1, int pageSize = 20)
        {
            return Ok(await _service.GetAll(page, pageSize));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserRoleResponseDto>> Get(int id)
        {
            var result = await _service.Get(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<UserRoleResponseDto>> Create(UserRoleCreateDto dto)
        {
            try { return Ok(await _service.Create(dto)); }
            catch (InvalidOperationException ex)
            { return BadRequest(new { error = ex.Message }); }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserRoleResponseDto>> Update(int id, UserRoleUpdateDto dto)
        {
            try { return Ok(await _service.Update(id, dto)); }
            catch (InvalidOperationException ex)
            { return BadRequest(new { error = ex.Message }); }
            catch (ArgumentException ex)
            { return NotFound(new { error = ex.Message }); }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                return Ok(await _service.Delete(id));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
