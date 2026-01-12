using Fleetly.Shared.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.ProtocolDtos
{
    public class ProtocolPhotoDto
    {
        [Required(ErrorMessage = "Identyfikator protokołu jest wymagany!")]
        public int ProtocolId { get; set; }

        [Required(ErrorMessage = "Lokalizacja uszkodzenia jest wymagana!")]
        public VehicleSide Side { get; set; } 

        public IFormFile Photo { get; set; } = null!;
    }
}