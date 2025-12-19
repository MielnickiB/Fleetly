using Fleetly.Shared.Dto.LocationDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Mappings
{
    public static class LocationMappings
    {
        public static LocationResponseDto ToLocationResponseDto(this Location l)
            => new()
            {
                Id = l.Id,
                UserId = l.UserId,
                UserFullName = l.User.Details.Name + " " + l.User.Details.Surname,
                City = l.City,
                Street = l.Street,
                BuildingNumber = l.BuildingNumber,
                ApartmentNumber = l.ApartmentNumber,
                PostalCode = l.PostalCode,
                IsPublic = l.IsPublic,
                Description = l.Description,
                IsActive = l.IsActive,
                CreatedAt = l.CreatedAt,
                UpdatedAt = l.UpdatedAt
            };
    }
}
