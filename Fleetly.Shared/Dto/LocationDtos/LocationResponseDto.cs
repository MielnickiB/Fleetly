namespace Fleetly.Shared.Dto.LocationDtos
{
    public class LocationResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string BuildingNumber { get; set; } = null!;
        public string? ApartmentNumber { get; set; }
        public string PostalCode { get; set; } = null!;
        public bool IsPublic { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
