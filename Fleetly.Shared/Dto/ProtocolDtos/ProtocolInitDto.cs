using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.ProtocolDtos
{
    public class ProtocolInitDto
    {
        [Required(ErrorMessage = "Identyfikator protokołu jest wymagany!")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Lokalizacja jest wymagana!")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Lokalizacja jest wymagana!")]
        public double Longitude { get; set; }
    }
}