using Fleetly.Shared.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.ProtocolDtos
{
    public class DamageCreateDto
    {
        public int ProtocolId { get; set; }

        [Required(ErrorMessage = "Lokalizacja uszkodzenia jest wymagana!")]
        public VehicleSide DamageSide { get; set; }
        [Required(ErrorMessage = "Informacja o cześci jest wymagana!")]
        public DamagePart DamagePart { get; set; }
        [Required(ErrorMessage = "Rodzaj uszkodzenia jest wyamgany!")]
        public DamageType DamageType { get; set; }

        public string? Description { get; set; }

        public IFormFile Photo { get; set; } = null!;
    }
}