using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.AvailabilityDtos
{
    public class AvailabilityCreateDto
    {
        [Required(ErrorMessage = "Data początkowa jest wymagana!")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "Data końcowa jest wymagana!")]
        public DateOnly EndDate { get; set; }

        [Required(ErrorMessage = "Godzina początkowa jest wymagana!")]
        public TimeOnly StartHour { get; set; }

        [Required(ErrorMessage = "Godzina końcowa jest wymagana!")]
        public TimeOnly EndHour { get; set; }
        public bool IsAvailable { get; set; }
    }

}
