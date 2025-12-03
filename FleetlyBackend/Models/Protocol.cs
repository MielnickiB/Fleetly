using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetlyBackend.Models
{
    public class Protocol : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int WorkerId { get; set; }
        public User Worker { get; set; } = null!;

        public int ClientId { get; set; }
        public User Client { get; set; } = null!;

        public int ProtocolTypeId { get; set; }
        public ProtocolType ProtocolType { get; set; } = null!;

        [StringLength(500)]
        public string? Notes { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Mileage { get; set; }

        [StringLength(200)]
        public string SignatureUrl { get; set; } = null!;
        [StringLength(200)]
        public string PdfUrl { get; set; } = null!;

        public bool HasRegistrationDocument { get; set; }
        public bool HasServiceBook { get; set; }
        public bool HasInsurancePolicy { get; set; }
        public int NumberOfKeys { get; set; }

        public bool IsActive { get; set; } = true;
        public ICollection<ProtocolPhoto> Photos { get; set; } = [];
    }
}