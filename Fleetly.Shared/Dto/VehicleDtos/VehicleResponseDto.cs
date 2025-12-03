namespace Fleetly.Shared.Dto.VehicleDtos
{
    public class VehicleResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BrandModelId { get; set; }
        public string RegistrationNumber { get; set; } = null!;
        public int? Mileage { get; set; }
        public string? VIN { get; set; }
        public int? Year { get; set; }
        public string? Details { get; set; }
        public bool IsActive { get; set; }
    }
}
