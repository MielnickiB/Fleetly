using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.OrderDtos
{
    public abstract class OrderBaseDto
    {
        [Required(ErrorMessage = "Pojazd jest wymagany!")]
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "Lokalizacja początkowa jest wymagana!")]
        public int StartLocationId { get; set; }

        [Required(ErrorMessage = "Lokalizacja końcowa jest wymagana!")]
        public int EndLocationId { get; set; }

        public int? ServiceLocationId { get; set; }

        [Required(ErrorMessage = "Dystans jest wymagany!")]
        [Range(1, 5000, ErrorMessage = "Dystans musi być większy od 0!")]
        public int RangeOfKm { get; set; }

        [Required(ErrorMessage = "Data rozpoczęcia jest wymagana!")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Data zakończenia jest wymagana!")]
        public DateTime Deadline { get; set; }

        public DateTime? ServiceTime { get; set; }

        public string? Details { get; set; }

        [Required(ErrorMessage = "Nazwa osoby kontaktowej jest wymagana!")]
        public string EndContactName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon osoby kontaktowej jest wymagany!")]
        [Phone(ErrorMessage = "Niepoprawny format telefonu!")]
        public string EndContactPhone { get; set; } = string.Empty;
    }
}
