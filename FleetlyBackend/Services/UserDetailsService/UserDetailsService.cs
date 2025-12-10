using Fleetly.Shared.Dto.UserDetailsDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Extensions;
using FleetlyBackend.Mappings;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.UserDetailsService
{
    public class UserDetailsService(FleetlyContext context, IHttpContextAccessor http) : IUserDetailsService
    {
        private readonly FleetlyContext _context = context;
        private readonly IHttpContextAccessor _http = http;

        public async Task<UserDetailsDto?> Get(int? userId)
        {
            var user = _http.CurrentUser();
            var id = userId ?? user.GetUserId();

            var details = await _context.UserDetails.FirstOrDefaultAsync(d => d.UserId == id);

            if (details is not null && details.UserId != user.GetUserId() && !user.IsAdmin())
                throw new UnauthorizedAccessException("Nie masz uprawnień do przeglądania tych szczegółów użytkownika.");

            return details?.ToDto();
        }

        public async Task<UserDetailsDto> Update(UserDetailsUpdateDto dto, int? userId)
        {
            var user = _http.CurrentUser();
            var id = userId ?? user.GetUserId();

            var details = await _context.UserDetails.FirstOrDefaultAsync(d => d.UserId == id);

            if (details is not null && details.UserId != user.GetUserId() && !user.IsAdmin())
                throw new UnauthorizedAccessException("Nie masz uprawnień do edytowania tych szczegółów użytkownika.");

            if (details is null)
                throw new ArgumentException("Nie znaleziono szczegółów użytkownika.");

            if (dto.Name is not null && dto.Name != details.Name) details.Name = dto.Name;
            if (dto.Surname is not null && dto.Surname != details.Surname) details.Surname = dto.Surname;
            if (dto.PhoneNumber is not null && dto.PhoneNumber != details.PhoneNumber) details.PhoneNumber = dto.PhoneNumber;
            if (dto.Company is not null && dto.Company != details.Company) details.Company = dto.Company;

            details.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return details.ToDto();
        }
    }
}
