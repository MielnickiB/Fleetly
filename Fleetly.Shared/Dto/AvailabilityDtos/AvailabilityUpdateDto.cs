namespace Fleetly.Shared.Dto.AvailabilityDtos
{
    public class AvailabilityUpdateDto
    {
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public TimeOnly? StartHour { get; set; }
        public TimeOnly? EndHour { get; set; }

        public bool? IsAvailable { get; set; }
    }

}
