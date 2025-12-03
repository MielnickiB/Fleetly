using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.CarBrandDtos
{
    public class CarBrandCreateDto
    {
        [Required]
        [StringLength(50)]
        public string BrandName { get; set; } = null!;
    }
}
