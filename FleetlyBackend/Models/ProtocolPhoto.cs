using System.ComponentModel.DataAnnotations;

namespace FleetlyBackend.Models
{
    public class ProtocolPhoto : BaseEntity
    {
        public int ProtocolId { get; set; }
        public Protocol Protocol { get; set; } = null!;

        [StringLength(200)]
        public string PhotoUrl { get; set; } = null!;
    }
}