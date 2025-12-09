using System.ComponentModel.DataAnnotations;
using Fleetly.Shared.Enums;

namespace Fleetly.Shared.Dto.VehicleDtos
{
    public class VehicleUpdateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "BrandModelId musi być większe od 0.")]
        public int? BrandModelId { get; set; }
        public string? RegistrationNumber { get; set; }
        public FuelType? FuelType { get; set; }
        public int? Mileage { get; set; }
        public string? VIN { get; set; }
        public int? Year { get; set; }
        public string? Details { get; set; }
    }
}
