using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.OrderDtos
{
    public class OrderUpdateDto
    {
        public int? VehicleId { get; set; }
        public int? WorkerId { get; set; }
        public int? StartLocationId { get; set; }
        public int? ServiceLocationId { get; set; }
        public int? EndLocationId { get; set; }
        public int? RangeOfKm { get; set; }
        public string? EndContactName { get; set; }
        [Phone(ErrorMessage = "Podaj poprawny format numeru telefonu")]
        public string? EndContactPhone { get; set; }
        public string? Details { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? ServiceTime { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
