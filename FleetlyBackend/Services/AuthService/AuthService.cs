using Fleetly.Shared.Dto.AuthDtos;
using Fleetly.Shared.Dto.UserDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.AuthService
{
    public class AuthService(FleetlyContext context, IPasswordHasher<User> hasher, ITokenService tokenService) : IAuthService
    {
        private readonly FleetlyContext _context = context;
        private readonly IPasswordHasher<User> _hasher = hasher;
        private readonly ITokenService _tokenService = tokenService;

        // Login zwraca token + refreshToken + zmapowanego usera
        public async Task<AuthResultDto?> LoginAsync(UserLoginDto request)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null) return null;

            var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verify == PasswordVerificationResult.Failed) throw new UnauthorizedAccessException("Nieprawidłowy email lub hasło");

            return CreateResponseToken(user);
        }

        // Rejestracja zwraca zmapowanego usera (bez hasha)
        public async Task<UserResponseDto?> RegisterAsync(UserRegisterDto request)
        {
            if (request is null) return null;
            if (request.Details is null) throw new ArgumentException("Szczegóły użytkownika są wymagane.");

            if (await _context.Users.AnyAsync(u => u.Email == request.Email)) throw new ArgumentException("Dany adres email jest już zajęty.");

            var user = new User
            {
                Email = request.Email
            };
            user.PasswordHash = _hasher.HashPassword(user, request.Password);

            var userRole = await _context.UserRoles.FirstOrDefaultAsync(r => r.RoleName == "Client") ?? throw new ArgumentException("Nie udało się pobrać podstawowej roli użytkownika.");
            user.RoleId = userRole.Id;

            var details = new UserDetails
            {
                Name = request.Details.Name,
                Surname = request.Details.Surname,
                PhoneNumber = request.Details.PhoneNumber,
                Company = request.Details.Company ?? string.Empty,
            };

            user.Details = details;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _context.Users
                .Include(u => u.Role)
                .FirstAsync(u => u.Id == user.Id);

            return user.ToResponseDto();
        }

        // Metoda pomocnicza do tworzenia odpowiedzi z tokenem i danymi użytkownika
        private AuthResultDto CreateResponseToken(User user)
        {
            return new AuthResultDto
            {
                AccessToken = _tokenService.CreateAccessToken(user),
                User = user.ToResponseDto()
            };
        }
    }
}