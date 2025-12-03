namespace Fleetly.Shared.Dto.AvailabilityDtos
{
    public class AvailabilityResponseDto
    {
        public int Id { get; set; }
        public int WorkerId { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public TimeOnly StartHour { get; set; }
        public TimeOnly EndHour { get; set; }

        public bool IsAvailable { get; set; }
    }

}
