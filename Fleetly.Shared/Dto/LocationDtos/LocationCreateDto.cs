using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.LocationDtos
{
    public class LocationCreateDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Miasto nie może przekraczać 100 znaków.")]
        public string City { get; set; } = null!;
        [Required]
        [StringLength(200, ErrorMessage = "Ulica nie może przekraczać 200 znaków.")]
        public string Street { get; set; } = null!;
        [Required]
        [StringLength(20, ErrorMessage = "Numer budynku nie może przekraczać 20 znaków.")]
        public string BuildingNumber { get; set; } = null!;
        [StringLength(20, ErrorMessage = "Numer mieszkania nie może przekraczać 20 znaków.")]
        public string? ApartmentNumber { get; set; }
        [Required]
        [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Kod pocztowy musi być w formacie XX-XXX.")]
        [StringLength(10, ErrorMessage = "Kod pocztowy nie może przekraczać 10 znaków.")]
        public string PostalCode { get; set; } = null!;
    }
}
