using System.ComponentModel.DataAnnotations;

namespace FleetlyBackend.Models
{
    public class Location : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [StringLength(100)]
        public string City { get; set; } = null!;    

        [StringLength(200)]
        public string Street { get; set; } = null!;

        [StringLength(20)]
        public string BuildingNumber { get; set; } = null!;

        [StringLength(20)]
        public string? ApartmentNumber { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
