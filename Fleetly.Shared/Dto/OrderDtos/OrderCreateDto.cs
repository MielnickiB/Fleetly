using Fleetly.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.OrderDtos
{
    public class OrderCreateDto
    {
        [Required]
        public OrderType Type { get; set; }
        [Required]
        public int VehicleId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int RangeOfKm { get; set; }
        public string? Details { get; set; }

        [Required]
        public int StartLocationId { get; set; }
        public int? ServiceLocationId { get; set; }
        [Required]
        public int EndLocationId { get; set; }

        public string? EndContactName { get; set; }
        [Phone(ErrorMessage = "Podaj poprawny format numeru telefonu")]
        public string? EndContactPhone { get; set; }

        [Required]
        public DateTime StartTime { get; set; }
        public DateTime? ServiceTime { get; set; }
        [Required]
        public DateTime Deadline { get; set; }
    }
}
