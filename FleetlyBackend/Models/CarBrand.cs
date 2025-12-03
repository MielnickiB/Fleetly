using System.ComponentModel.DataAnnotations;

namespace FleetlyBackend.Models
{
    public class CarBrand : BaseEntity
    {
        [StringLength(50)]
        public string BrandName { get; set; } = null!;

        public ICollection<BrandModel> Models { get; set; } = [];

        public bool IsActive { get; set; } = true;
    }
}