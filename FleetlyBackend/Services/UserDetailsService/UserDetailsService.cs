using Fleetly.Shared.Dto.UserDetailsDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Mappings;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.UserDetailsService
{
    public class UserDetailsService(FleetlyContext context) : IUserDetailsService
    {
        private readonly FleetlyContext _context = context;

        public async Task<UserDetailsDto?> Get(int userId)
        {
            var details = await _context.UserDetails
                .FirstOrDefaultAsync(d => d.UserId == userId);

            return details?.ToDto();
        }

        public async Task<UserDetailsDto> Update(int userId, UserDetailsUpdateDto dto)
        {
            var details = await _context.UserDetails
                .FirstOrDefaultAsync(d => d.UserId == userId)
                ?? throw new InvalidOperationException("Nie znaleziono danych użytkownika.");

            if (dto.Name is not null && dto.Name != details.Name) details.Name = dto.Name; 
            if (dto.Surname is not null && dto.Surname != details.Surname)  details.Surname = dto.Surname; 
            if (dto.PhoneNumber is not null && dto.PhoneNumber != details.PhoneNumber) details.PhoneNumber = dto.PhoneNumber;
            if (dto.Company is not null && dto.Company != details.Company) details.Company = dto.Company;

            details.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return details.ToDto();
        }
    }
}
