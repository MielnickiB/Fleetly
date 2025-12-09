using Fleetly.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace FleetlyBackend.Models
{
    public class Vehicle : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int BrandModelId { get; set; }
        public BrandModel BrandModel { get; set; } = null!;

        [StringLength(10)]
        public string RegistrationNumber { get; set; } = null!;

        public int? Mileage { get; set; }

        [StringLength(17)]
        public string? VIN { get; set; }

        public int? Year { get; set; }

        [StringLength(100)]
        public string? Details { get; set; }

        public FuelType FuelType { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Order>? Orders { get; set; }
        public ICollection<Damage>? Damages { get; set; }
    }
}