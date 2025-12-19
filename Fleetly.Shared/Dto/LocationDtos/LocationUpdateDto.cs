using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.LocationDtos
{
    public class LocationUpdateDto
    {
        [StringLength(100, ErrorMessage = "Miasto nie może przekraczać 100 znaków.")]
        public string? City { get; set; }

        [StringLength(200, ErrorMessage = "Ulica nie może przekraczać 200 znaków.")]
        public string? Street { get; set; }

        [StringLength(20, ErrorMessage = "Numer budynku nie może przekraczać 20 znaków.")]
        public string? BuildingNumber { get; set; }

        [StringLength(20, ErrorMessage = "Numer mieszkania nie może przekraczać 20 znaków.")]
        public string? ApartmentNumber { get; set; }

        [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Kod pocztowy musi być w formacie XX-XXX.")]
        [StringLength(10, ErrorMessage = "Kod pocztowy nie może przekraczać 10 znaków.")]
        public string? PostalCode { get; set; }

        public bool IsPublic { get; set; }

        [StringLength(500, ErrorMessage = "Opis nie może przekraczać 500 znaków.")]
        public string? Description { get; set; }
    }
}
