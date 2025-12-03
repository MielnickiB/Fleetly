using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetlyBackend.Models
{
    public class UserDetails : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [StringLength(50)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string Surname { get; set; } = null!;

        [StringLength(30)]
        public string PhoneNumber { get; set; } = null!;

        [StringLength(50)]
        public string? Company { get; set; }
    }
}