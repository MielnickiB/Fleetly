using Fleetly.Shared.Dto.AvailabilityDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Mappings
{
    public static class AvailabilityMappings
    {
        public static AvailabilityResponseDto ToResponseDto(this Availability a)
            => new()
            {
                Id = a.Id,
                WorkerId = a.WorkerId,
                Date = a.Date,
                StartHour = a.StartHour,
                EndHour = a.EndHour,
                IsAvailable = a.IsAvailable
            };
    }

}
