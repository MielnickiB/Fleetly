using Fleetly.Shared.Enums;

namespace Fleetly.Shared.Dto.ProtocolDtos
{
    public class ProtocolPhotoResponseDto
    {
        public int Id { get; set; }
        public int ProtocolId { get; set; }
        public VehicleSide Side { get; set; }
        public string PhotoUrl { get; set; } = null!;
    }
}
