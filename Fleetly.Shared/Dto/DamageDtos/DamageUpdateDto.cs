using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Fleetly.Shared.Dto.DamageDtos
{
    public class DamageUpdateDto
    {
        [StringLength(50)]
        public string? DamageSide { get; set; }
        [StringLength(50)]
        public string? DamageLocation { get; set; }
        [StringLength(100)]
        public string? DamagePart { get; set; }
        [StringLength(50)]
        public string? DamageType { get; set; }
        [StringLength(300)]
        public string? Description { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
