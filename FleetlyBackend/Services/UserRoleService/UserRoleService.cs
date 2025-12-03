using Fleetly.Shared.Dto.UserRoleDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Helpers;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.UserRoleService
{
    public class UserRoleService(FleetlyContext context) : IUserRoleService
    {
        private readonly FleetlyContext _context = context;

        public async Task<List<UserRoleResponseDto>> GetAll(int page, int pageSize)
        {
            var (skip, take) = PaginationHelper.Calculate(page, pageSize);

            return await _context.UserRoles
                .AsNoTracking()
                .OrderBy(r => r.RoleName)
                .Skip(skip)
                .Take(take)
                .Select(r => r.ToResponseDto())
                .ToListAsync();
        }

        public async Task<UserRoleResponseDto?> Get(int id)
        {
            var role = await _context.UserRoles.FindAsync(id);
            return role?.ToResponseDto();
        }

        public async Task<UserRoleResponseDto> Create(UserRoleCreateDto dto)
        {
            var exists = await _context.UserRoles
                .AnyAsync(r => r.RoleName.ToLower() == dto.RoleName.ToLower());

            if (exists)
                throw new InvalidOperationException("Rola o takiej nazwie już istnieje.");

            var role = new UserRole
            {
                RoleName = dto.RoleName
            };

            _context.UserRoles.Add(role);
            await _context.SaveChangesAsync();

            return role.ToResponseDto();
        }

        public async Task<UserRoleResponseDto> Update(int id, UserRoleUpdateDto dto)
        {
            var role = await _context.UserRoles.FindAsync(id)
                ?? throw new ArgumentException("Nie znaleziono roli.");

            var exists = await _context.UserRoles
                .AnyAsync(r => r.RoleName.ToLower() == dto.RoleName.ToLower() && r.Id != id);

            if (exists)
                throw new InvalidOperationException("Rola o takiej nazwie już istnieje.");

            role.RoleName = dto.RoleName;
            role.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return role.ToResponseDto();
        }

        public async Task<bool> Delete(int id)
        {
            var role = await _context.UserRoles.FindAsync(id) ?? throw new ArgumentException("Nie znaleziono danej roli.");

            var hasUsers = await _context.Users.AnyAsync(u => u.RoleId == id);
            if (hasUsers)
                throw new InvalidOperationException("Nie można usunąć roli, która jest przypisana do użytkowników.");

            _context.UserRoles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
