using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fleetly.Shared.Enums;

namespace FleetlyBackend.Models
{
    public class Protocol : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public int WorkerId { get; set; }
        public User Worker { get; set; } = null!;

        public int ClientId { get; set; }
        public User Client { get; set; } = null!;

        public ProtocolType Type { get; set; }
        public int CurrentStep { get; set; } = 1;

        public double LocationLatitude { get; set; }
        public double LocationLongitude { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Mileage { get; set; }
        public decimal FuelLevel { get; set; }

        [StringLength(200)]
        public string? SignatureUrl { get; set; }
        [StringLength(200)]
        public string? PdfUrl { get; set; }

        public bool HasRegistrationDocument { get; set; }
        public bool HasServiceBook { get; set; }
        public bool HasInsurancePolicy { get; set; }
        public int NumberOfKeys { get; set; }

        public ICollection<ProtocolPhoto> Photos { get; set; } = [];
        public ICollection<Damage> Damages { get; set; } = [];
    }
}