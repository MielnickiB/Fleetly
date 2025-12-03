using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.VehicleDtos
{
    public class VehicleCreateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "BrandModelId musi być większe od 0.")]
        public int BrandModelId { get; set; }

        [Required]
        public string RegistrationNumber { get; set; } = null!;

        public int? Mileage { get; set; }
        public string? VIN { get; set; }
        public int? Year { get; set; }
        public string? Details { get; set; }
    }

}
