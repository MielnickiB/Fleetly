using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.BrandModelDtos
{
    public class BrandModelCreateDto
    {
        [Required]
        [StringLength(50)]
        public string ModelName { get; set; } = null!;
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "CarBrandId musi być większe od 0.")]
        public int CarBrandId { get; set; }
    }
}
