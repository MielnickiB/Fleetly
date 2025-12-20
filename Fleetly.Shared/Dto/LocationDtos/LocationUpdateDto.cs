using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.LocationDtos
{
    public class LocationUpdateDto
    {
        [Required(ErrorMessage = "Miasto jest wymagane!")]
        [StringLength(100, ErrorMessage = "Miasto nie może przekraczać 100 znaków!")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Ulica jest wymagana!")]
        [StringLength(200, ErrorMessage = "Ulica nie może przekraczać 200 znaków!")]
        public string Street { get; set; } = null!;

        [Required(ErrorMessage = "Numer budynku jest wymagany!")]
        [StringLength(20, ErrorMessage = "Numer budynku nie może przekraczać 20 znaków!")]
        public string BuildingNumber { get; set; } = null!;
        [StringLength(20, ErrorMessage = "Numer mieszkania nie może przekraczać 20 znaków!")]
        public string? ApartmentNumber { get; set; }
        [Required(ErrorMessage = "Kod pocztowy jest wymagany!")]
        [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Kod pocztowy musi być w formacie XX-XXX!")]
        [StringLength(10, ErrorMessage = "Kod pocztowy nie może przekraczać 10 znaków!")]
        public string PostalCode { get; set; } = null!;

        public bool IsPublic { get; set; } = false;

        [StringLength(500, ErrorMessage = "Opis nie może przekraczać 500 znaków!")]
        public string? Description { get; set; }
    }
}
