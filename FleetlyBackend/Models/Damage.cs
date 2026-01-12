using Fleetly.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace FleetlyBackend.Models
{
    public class Damage : BaseEntity
    {
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public int? ProtocolId { get; set; }
        public Protocol? Protocol { get; set; }

        public VehicleSide Side { get; set; }
        public DamagePart Part { get; set; }
        public DamageType Type { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string PhotoUrl { get; set; } = null!;
    }
}