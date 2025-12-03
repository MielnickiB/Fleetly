using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Fleetly.Shared.Dto.DamageDtos
{
    public class DamageCreateDto
    {
        [Required]
        public int VehicleId { get; set; }
        [Required]
        public int ProtocolId { get; set; }

        [Required]
        [StringLength(50)]

        public string DamageSide { get; set; } = null!;
        [Required] 
        [StringLength(50)]
        public string DamageLocation { get; set; } = null!;
        [Required] 
        [StringLength(100)] 
        public string DamagePart { get; set; } = null!;
        [Required] 
        [StringLength(50)] 
        public string DamageType { get; set; } = null!;
        [StringLength(300)]
        public string? Description { get; set; }

        [Required]
        public IFormFile Photo { get; set; } = null!;
    }
}
