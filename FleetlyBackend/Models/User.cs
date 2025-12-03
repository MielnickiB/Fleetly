using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FleetlyBackend.Models
{
    public class User : BaseEntity
    {
        public int RoleId { get; set; }
        public UserRole Role { get; set; } = null!;

        [StringLength(100)]
        public string Email { get; set; } = null!;

        [JsonIgnore]
        [StringLength(200)]
        public string PasswordHash { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public UserDetails Details { get; set; } = null!;

        public ICollection<Location> Locations { get; set; } = [];
        public ICollection<Notification> Notifications { get; set; } = [];
        public ICollection<Availability> WorkerAvailabilities { get; set; } = [];
        public ICollection<Vehicle> Vehicles { get; set; } = [];
        public ICollection<Order> ClientOrders { get; set; } = [];
        public ICollection<Order> WorkerOrders { get; set; } = [];
    }
}
