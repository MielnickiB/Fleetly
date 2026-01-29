using Fleetly.Shared.Dto.ExpenseDtos;
using Fleetly.Shared.Dto.LocationDtos;
using Fleetly.Shared.Dto.VehicleDtos;
using Fleetly.Shared.Dto.UserDtos;
using Fleetly.Shared.Dto.ProtocolDtos;
using Fleetly.Shared.Enums;

namespace Fleetly.Shared.Dto.OrderDtos
{
    public class OrderResponseDto
    {
        public int Id { get; set; }

        public UserResponseDto Client { get; set; } = null!;
        public UserResponseDto? Worker { get; set; }
        public VehicleResponseDto Vehicle { get; set; } = null!;

        public LocationResponseDto StartLocation { get; set; } = null!;
        public LocationResponseDto EndLocation { get; set; } = null!;

        public OrderStatus Status { get; set; }
        public string? Details { get; set; }

        public int RangeOfKm { get; set; }
        public decimal Salary { get; set; }
        public decimal AdditionalCosts { get; set; }
        public decimal FuelCosts { get; set; }
        public decimal TotalCosts => (Salary * 0.3m) + Salary + AdditionalCosts + FuelCosts;
        public decimal MaxNonFuelCosts { get; set; }
        public decimal MaxSalary { get; set; }
        public decimal MaxProjectedValue => MaxSalary * 0.3m + MaxSalary + MaxNonFuelCosts;

        public List<ExpenseResponseDto> Expenses { get; set; } = [];
        public List<ProtocolLiteDto> Protocols { get; set; } = [];

        public string EndContactName { get; set; } = null!;
        public string EndContactPhone { get; set; } = null!;

        public DateTime StartTime { get; set; }
        public DateTime Deadline { get; set; }

        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
