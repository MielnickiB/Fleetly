using Fleetly.Shared.Dto.UserDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Helpers;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.UserSerivce
{
    public class UserService(FleetlyContext context, IPasswordHasher<User> hasher) : IUserService
    {
        private readonly FleetlyContext _context = context;
        private readonly IPasswordHasher<User> _hasher = hasher;

        public async Task<List<UserResponseDto>> GetAll(int page = 1, int pageSize = 10)
        {
            var (skip, safe) = PaginationHelper.Calculate(page, pageSize);

            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .OrderBy(u => u.Id)
                .Skip(skip)
                .Take(safe)
                .Select(u => u.ToResponseDto())
                .ToListAsync();
        }

        public async Task<List<UserResponseDto>> GetAllByRole(string roleName, int page, int pageSize = 10)
        {
            var (skip, safe) = PaginationHelper.Calculate(page, pageSize);

            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == roleName)
                .OrderBy(u => u.Id)
                .Skip(skip)
                .Take(safe)
                .Select(u => u.ToResponseDto())
                .ToListAsync();
        }

        public async Task<UserResponseDto?> GetById(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            return user?.ToResponseDto();
        }
        public async Task<UserResponseDto?> GetCurrent(int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            return user?.ToResponseDto();
        }

        public async Task<UserResponseDto?> Create(UserCreateDto newUser)
        {
            ArgumentNullException.ThrowIfNull(newUser);
            if (newUser.Details is null) throw new ArgumentException("Szczegóły użytkownika są wymagane.");

            if (await _context.Users.AnyAsync(u => u.Email == newUser.Email)) throw new ArgumentException("Dany adres email jest już zajęty.");

            var user = new User
            {
                Email = newUser.Email
            };
            user.PasswordHash = _hasher.HashPassword(user, newUser.Password);

            if (newUser.RoleId <= 0)
            {
                throw new ArgumentException("Id roli musi być większe od 0");
            }
            if (!await _context.UserRoles.AnyAsync(r => r.Id == newUser.RoleId))
            {
                throw new ArgumentException("Podana rola nie istnieje");
            }

            user.RoleId = newUser.RoleId;

            var details = new UserDetails
            {
                Name = newUser.Details.Name,
                Surname = newUser.Details.Surname,
                PhoneNumber = newUser.Details.PhoneNumber,
                Company = newUser.Details.Company ?? string.Empty,
            };

            user.Details = details;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _context.Users
                .Include(u => u.Role)
                .FirstAsync(u => u.Id == user.Id);

            return user.ToResponseDto();
        }

        public async Task<UserResponseDto> Update(int id, UserUpdateDto dto)
        {
            var u = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new ArgumentException("Nie znaleziono użytkownika");

            if (!u.IsActive)
                throw new InvalidOperationException("Nie można edytować nieaktywnego użytkownika");

            if (dto.Email is not null && dto.Email != u.Email)
            {
                var emailInUse = await _context.Users.AnyAsync(x => x.Email == dto.Email && x.Id != u.Id);
                if (emailInUse)
                    throw new ArgumentException("Podany adres email jest już używany przez innego użytkownika");

                u.Email = dto.Email;
            }

            if (dto.RoleId is not null && dto.RoleId != u.RoleId)
            {
                var roleExists = await _context.UserRoles.AnyAsync(r => r.Id == dto.RoleId);
                if (!roleExists)
                    throw new ArgumentException("Podana rola nie istnieje");

                u.RoleId = (int)dto.RoleId;
            }

            u.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var updatedUser = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Where(u => u.Id == id)
                .FirstAsync();

            return updatedUser.ToResponseDto();
        }

        public async Task<bool> Deactivate(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id) ?? throw new ArgumentException("Nie znaleziono użytkownika");

            if (!user.IsActive) throw new InvalidOperationException("Użytkownik jest już nieaktywny");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
