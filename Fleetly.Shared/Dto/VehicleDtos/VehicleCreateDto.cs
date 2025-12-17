using Fleetly.Shared.Enums;
using Fleetly.Shared.Validations;
using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.VehicleDtos
{
    public class VehicleCreateDto
    {
        [Required(ErrorMessage = "Model jest wymagany")]
        [Range(1, int.MaxValue, ErrorMessage = "Nieprawidłowy model")]
        public int BrandModelId { get; set; }

        [Required(ErrorMessage = "Wymagane jest podanie numeru rejestracyjnego")]
        public string RegistrationNumber { get; set; } = null!;

        [Required(ErrorMessage = "Typ paliwa jest wymagany")]
        public FuelType FuelType { get; set; }

        [Required(ErrorMessage = "Przebieg jest wymagany")]
        [Range(0, 999999, ErrorMessage = "Przebieg musi być nieujemny i nie większy od miliona.")]
        public int Mileage { get; set; }
        public string? VIN { get; set; }
        [Required(ErrorMessage = "Rok produkcji jest wymagany")]
        [VehicleYearValidation(minYearsBack: 30)]
        public int Year { get; set; }
        public string? Details { get; set; }
    }
}
