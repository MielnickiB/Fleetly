namespace Fleetly.Shared.Dto.DamageDtos
{
    public class DamageResponseDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int ProtocolId { get; set; }
        public string DamageSide { get; set; } = null!;
        public string DamageLocation { get; set; } = null!;
        public string DamagePart { get; set; } = null!;
        public string DamageType { get; set; } = null!;
        public string? Description { get; set; }
        public string PhotoUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
