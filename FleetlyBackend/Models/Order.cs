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

        public string? Details { get; set; }

        public int StartLocationId { get; set; }
        public Location StartLocation { get; set; } = null!;

        public int EndLocationId { get; set; }
        public Location EndLocation { get; set; } = null!;

        public int RangeOfKm { get; set; }

        public decimal Salary { get; set; }

        public decimal AdditionalCosts { get; set; } = 0m;
        public decimal FuelCosts { get; set; } = 0m;
        public List<Expense> Expenses { get; set; } = [];

        public string EndContactName { get; set; } = null!;
        public string EndContactPhone { get; set; } = null!;

        public DateTime StartTime { get; set; }
        public DateTime Deadline { get; set; }

        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }

        public bool IsActive { get; set; } = true;
    }
}