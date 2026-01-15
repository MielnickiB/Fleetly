using Fleetly.Shared.Enums;

namespace Fleetly.Shared.Dto.DamageDtos
{
    public class DamageResponseDto
    {
        public int Id { get; set; }
        public VehicleSide Side { get; set; }
        public DamagePart Part { get; set; }
        public DamageType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = null!;

        public bool IsNew { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsFixed { get; set; } = false;
        public DateTime? FixedAt { get; set; }
        public int? FixedByProtocolId { get; set; }
    }
}
