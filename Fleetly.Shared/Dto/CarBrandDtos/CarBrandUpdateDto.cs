using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.CarBrandDtos
{
    public class CarBrandUpdateDto
    {
        [StringLength(50)]
        public string? BrandName { get; set; }
    }
}
