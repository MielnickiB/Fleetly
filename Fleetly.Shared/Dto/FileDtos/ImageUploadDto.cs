using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Fleetly.Shared.Dto.FileDtos
{
    public class ImageUploadDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
