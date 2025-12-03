using System.ComponentModel.DataAnnotations;

namespace FleetlyBackend.Models
{
    public class Damage : BaseEntity
    {
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public int ProtocolId { get; set; }
        public Protocol Protocol { get; set; } = null!;

        [StringLength(50)]
        public string DamageSide { get; set; } = null!;
        [StringLength(100)]
        public string DamagePart { get; set; } = null!;
        [StringLength(50)]
        public string DamageLocation { get; set; } = null!;
        [StringLength(50)]
        public string DamageType { get; set; } = null!;
        [StringLength(300)]
        public string? Description { get; set; }
        [StringLength(200)]
        public string PhotoUrl { get; set; } = null!;
    }
}