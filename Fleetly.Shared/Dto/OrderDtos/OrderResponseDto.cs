using Fleetly.Shared.Dto.ExpenseDtos;
using Fleetly.Shared.Enums;

namespace Fleetly.Shared.Dto.OrderDtos
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int? WorkerId { get; set; }
        public string? WorkerName { get; set; }
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string VehicleRegistrationNumber { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public OrderType Type { get; set; }
        public string? Details { get; set; }

        public int StartLocationId { get; set; }
        public string StartLocationCity { get; set; } = string.Empty;
        public string StartLocationAddress { get; set; } = string.Empty;
        public int? ServiceLocationId { get; set; }
        public string? ServiceLocationCity { get; set; }
        public string? ServiceLocationAddress { get; set; } = string.Empty;
        public int EndLocationId { get; set; }
        public string EndLocationCity { get; set; } = string.Empty;
        public string EndLocationAddress { get; set; } = string.Empty;
        public int RangeOfKm { get; set; }

        public decimal Salary { get; set; }
        public decimal AdditionalCosts { get; set; }
        public decimal FuelCosts { get; set; }
        public List<ExpenseResponseDto> Expenses { get; set; } = [];

        public DateTime StartTime { get; set; }
        public DateTime? ServiceTime { get; set; }
        public DateTime Deadline { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
