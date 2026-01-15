using System.ComponentModel.DataAnnotations;
using Fleetly.Shared.Enums;

namespace FleetlyBackend.Models
{
    public class ProtocolPhoto : BaseEntity
    {
        public int ProtocolId { get; set; }
        public Protocol Protocol { get; set; } = null!;

        public VehicleSide Side { get; set; }

        [StringLength(200)]
        public string PhotoUrl { get; set; } = null!;
    }
}