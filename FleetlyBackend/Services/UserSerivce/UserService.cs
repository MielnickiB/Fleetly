using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.UserDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Extensions;
using FleetlyBackend.Helpers;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.UserSerivce
{
    public class UserService(FleetlyContext context, IPasswordHasher<User> hasher, IHttpContextAccessor http) : IUserService
    {
        private readonly FleetlyContext _context = context;
        private readonly IPasswordHasher<User> _hasher = hasher;
        private readonly IHttpContextAccessor _http = http;

        public async Task<PagedResult<UserResponseDto>> GetAll()
        {
            var totalCount = await _context.Users.CountAsync();
            var users = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Details)
                .OrderBy(u => u.Id)
                .Select(u => u.ToResponseDto())
                .ToListAsync();
            return new PagedResult<UserResponseDto>
            {
                Items = users,
                TotalCount = totalCount
            };
        }

        public async Task<List<UserResponseDto>> GetAllByRole(string roleName)
        {
            try
            {
                return await _context.Users
                    .AsNoTracking()
                    .Include(u => u.Role)
                    .Include(u => u.Details)
                    .Where(u => u.Role.RoleName == roleName)
                    .OrderBy(u => u.Id)
                    .Select(u => u.ToResponseDto())
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Wystąpił błąd podczas pobierania użytkowników o podanej roli.", ex);
            }
        }

        public async Task<UserResponseDto?> GetById(int id)
        {
            if (id <= 0) throw new ArgumentException("Id użytkownika musi być większe od 0");

            var actionUser = _http.CurrentUser();
            var userId = actionUser.GetUserId();

            var query = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Details)
                .AsQueryable();

            if (!actionUser.IsAdmin())
            {
                query = query.Where(u => u.Id == userId);
            }
            else
            {
                query = query.Where(u => u.Id == id);
            }

            var user = await query.FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Nie znaleziono użytkownika");

            return user.ToResponseDto();
        }

        public async Task<UserResponseDto?> Create(UserCreateDto newUser)
        {
            ArgumentNullException.ThrowIfNull(newUser);
            if (newUser.Details is null) throw new ArgumentException("Szczegóły użytkownika są wymagane.");

            if (await _context.Users.AnyAsync(u => u.Email == newUser.Email && u.IsActive)) throw new ArgumentException("Dany adres email jest już zajęty.");

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
            var created = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Details)
                .FirstOrDefaultAsync(u => u.Id == user.Id)
                ?? throw new InvalidOperationException("Nie udało się pobrać utworzonego użytkownika");

            return created.ToResponseDto();
        }

        public async Task<UserResponseDto> Update(int id, UserUpdateDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if (id <= 0) throw new ArgumentException("Id użytkownika musi być większe od 0");

            var user = _http.CurrentUser();
            var u = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Details)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (u is not null && user.GetUserId() != u.Id && !user.IsInRole("Admin"))
                throw new UnauthorizedAccessException("Nie masz uprawnień do edytowania tego użytkownika");

            if (u is null)
                throw new ArgumentException("Nie znaleziono użytkownika");

            if (!u.IsActive)
                throw new InvalidOperationException("Nie można edytować nieaktywnego użytkownika");

            if (dto.Email is not null && !dto.Email.Equals(u.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailInUse = await _context.Users.AnyAsync(x => x.Email == dto.Email && x.Id != u.Id && x.IsActive);
                if (emailInUse)
                    throw new ArgumentException("Podany adres email jest już używany przez innego użytkownika");

                u.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                u.PasswordHash = _hasher.HashPassword(u, dto.Password);
            }

            if (dto.RoleId != u.RoleId)
            {
                var roleExists = await _context.UserRoles.AnyAsync(r => r.Id == dto.RoleId);
                if (!roleExists)
                    throw new ArgumentException("Podana rola nie istnieje");

                u.RoleId = (int)dto.RoleId;
            }

            if (dto.Details is not null && u.Details is not null)
            {
                var detailsDto = dto.Details;
                if (detailsDto.Name is not null && detailsDto.Name != string.Empty && !detailsDto.Name.Equals(u.Details.Name, StringComparison.OrdinalIgnoreCase))
                    u.Details.Name = detailsDto.Name;
                if (detailsDto.Surname is not null && detailsDto.Surname != string.Empty && !detailsDto.Surname.Equals(u.Details.Surname, StringComparison.OrdinalIgnoreCase))
                    u.Details.Surname = detailsDto.Surname;
                if (detailsDto.PhoneNumber is not null)
                {
                    var newPhone = detailsDto.PhoneNumber.Trim();
                    var currentPhone = u.Details.PhoneNumber?.Trim();
                    if (!string.Equals(newPhone, currentPhone, StringComparison.OrdinalIgnoreCase))
                        u.Details.PhoneNumber = newPhone;
                }
                if (detailsDto.Company is not null && detailsDto.Company != string.Empty && !detailsDto.Company.Equals(u.Details.Company, StringComparison.OrdinalIgnoreCase))
                    u.Details.Company = detailsDto.Company;
            }

            u.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var updatedUser = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Details)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync()
                ?? throw new InvalidOperationException("Nie udało się pobrać zaaktualizowanego użytkownika");

            return updatedUser.ToResponseDto();
        }

        public async Task<bool> Deactivate(int id)
        {
            if (id <= 0) throw new ArgumentException("Id użytkownika musi być większe od 0");
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
