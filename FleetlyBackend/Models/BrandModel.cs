using System.ComponentModel.DataAnnotations;

namespace FleetlyBackend.Models
{
    public class BrandModel : BaseEntity
    {
        [StringLength(50)]
        public required string ModelName { get; set; }

        public int CarBrandId { get; set; }
        public CarBrand CarBrand { get; set; } = null!;

        public ICollection<Vehicle> Vehicles { get; set; } = [];

        public bool IsActive { get; set; } = true;
    }
}