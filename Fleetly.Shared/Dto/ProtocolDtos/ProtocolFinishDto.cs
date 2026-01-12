using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.ProtocolDtos
{
    public class ProtocolFinishDto
    {
        [Required(ErrorMessage = "Identyfikator protokołu jest wymagany!")]
        public int ProtocolId { get; set; }

        [Required(ErrorMessage = "Przebieg pojazdu jest wymagany!")]
        [Range(0, double.MaxValue)]
        public decimal Mileage { get; set; }

        [Required(ErrorMessage = "Stan paliwa pojazdu jest wymagany!")]
        [Range(0, 100)]
        public decimal FuelLevel { get; set; }

        public string? Notes { get; set; }

        public bool HasRegistrationDocument { get; set; }
        public bool HasServiceBook { get; set; }
        public bool HasInsurancePolicy { get; set; }
        public int NumberOfKeys { get; set; }

        public IFormFile SignaturePhoto { get; set; } = null!;
    }
}