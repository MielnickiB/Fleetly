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
                City = l.City,
                Street = l.Street,
                BuildingNumber = l.BuildingNumber,
                ApartmentNumber = l.ApartmentNumber,
                PostalCode = l.PostalCode
            };
    }
}
