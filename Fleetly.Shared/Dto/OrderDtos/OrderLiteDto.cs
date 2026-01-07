using Fleetly.Shared.Enums;

namespace Fleetly.Shared.Dto.OrderDtos
{
    public class OrderLiteDto
    {
        public int Id { get; set; }
        public string Car { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string StartCity { get; set; } = string.Empty;
        public string EndCity { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public OrderStatus Status { get; set; }
    }
}
