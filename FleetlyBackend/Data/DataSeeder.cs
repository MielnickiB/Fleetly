using FleetlyBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(FleetlyContext context, IPasswordHasher<User> passwordHasher)
        {
            await context.Database.EnsureCreatedAsync();

            if (!await context.UserRoles.AnyAsync())
            {
                var roles = new List<UserRole>
                {
                    new() { RoleName = "Admin" },
                    new() { RoleName = "Client" },
                    new() { RoleName = "Worker" }
                };

                await context.UserRoles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }

            if (!await context.Users.AnyAsync(u => u.Email == "user@example.com"))
            {
                var adminRole = await context.UserRoles.FirstAsync(r => r.RoleName == "Admin");

                var admin = new User
                {
                    Email = "user@example.com",
                    RoleId = adminRole.Id,
                    IsActive = true,
                    PasswordHash = string.Empty,
                    Details = new UserDetails
                    {
                        Name = "System",
                        Surname = "Administrator",
                        PhoneNumber = "000000000",
                        Company = "Fleetly"
                    }
                };

                admin.PasswordHash = passwordHasher.HashPassword(admin, "string");

                await context.Users.AddAsync(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}
