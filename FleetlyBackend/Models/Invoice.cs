using Fleetly.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetlyBackend.Models
{
    public class Invoice : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Sum { get; set; }

        public bool IsPaid { get; set; }

        public DateTime? DateOfPayment { get; set; }

        public DateTime DueDate { get; set; }

        public MethodOfPayment? MethodOfPayment { get; set; }
        public string? StripeSessionId { get; set; }
        [Timestamp]
        public byte[] Version { get; set; }
    }
}