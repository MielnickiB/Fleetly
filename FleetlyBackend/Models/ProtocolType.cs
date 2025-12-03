using System.ComponentModel.DataAnnotations;

namespace FleetlyBackend.Models
{
    public class ProtocolType : BaseEntity
    {
        [StringLength(100)]
        public string TypeName { get; set; } = null!;

        public ICollection<Protocol>? Protocols { get; set; }
    }
}