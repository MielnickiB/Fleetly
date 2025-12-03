using Fleetly.Shared.Enums;

namespace FleetlyBackend.Models
{
    public class Order : BaseEntity
    {
        public int ClientId { get; set; }
        public User Client { get; set; } = null!;

        public int? WorkerId { get; set; }
        public User? Worker { get; set; }

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public OrderStatus Status { get; set; } = OrderStatus.PendingApproval;

        public OrderType Type { get; set; }

        public string? Details { get; set; }

        public int StartLocationId { get; set; }
        public Location StartLocation { get; set; } = null!;

        public int? ServiceLocationId { get; set; }
        public Location? ServiceLocation { get; set; }

        public int EndLocationId { get; set; }
        public Location EndLocation { get; set; } = null!;

        public int RangeOfKm { get; set; }

        public decimal Salary { get; set; }

        public decimal AdditionalCosts { get; set; } = 0m;
        public decimal FuelCosts { get; set; } = 0m;
        public List<Expense> Expenses { get; set; } = [];

        public DateTime StartTime { get; set; }
        public DateTime? ServiceTime { get; set; }
        public DateTime Deadline { get; set; }

        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualArrivedServiceTime { get; set; }
        public DateTime? ActualLeftServiceTime { get; set; }
        public DateTime? ActualEndTime { get; set; }

        public bool IsActive { get; set; } = true;
    }
}