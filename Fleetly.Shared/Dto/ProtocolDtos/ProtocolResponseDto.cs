using Fleetly.Shared.Enums;
using Fleetly.Shared.Dto.DamageDtos;

namespace Fleetly.Shared.Dto.ProtocolDtos
{
    public class ProtocolResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public ProtocolType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CurrentStep { get; set; }

        public decimal Mileage { get; set; }
        public decimal FuelLevel { get; set; }
        public bool HasRegistrationDocument { get; set; }
        public bool HasInsurancePolicy { get; set; }
        public int NumberOfKeys { get; set; }
        public string? SignatureUrl { get; set; }

        public List<ProtocolPhotoResponseDto> Photos { get; set; } = [];
        public List<DamageResponseDto> Damages { get; set; } = [];
    }
}