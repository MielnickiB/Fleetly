using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.AvailabilityDtos
{
    public class AvailabilityCreateDto
    {
        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        public TimeOnly StartHour { get; set; }

        [Required]
        public TimeOnly EndHour { get; set; }
        public bool IsAvailable { get; set; }
    }

}
