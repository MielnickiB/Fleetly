using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetlyBackend.Models
{
    public class UserRole : BaseEntity
    {
        [StringLength(30)]
        public string RoleName { get; set; } = null!;

        public ICollection<User> Users { get; set; } = [];
    }
}