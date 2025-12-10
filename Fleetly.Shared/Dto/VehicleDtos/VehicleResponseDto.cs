using Fleetly.Shared.Enums;
using Fleetly.Shared.Dto.UserDetailsDtos;
using Fleetly.Shared.Dto.BrandModelDtos;

namespace Fleetly.Shared.Dto.VehicleDtos
{
    public class VehicleResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserDetailsDto UserDetails { get; set; } = null!;
        public int BrandModelId { get; set; }
        public BrandModelResponseDto BrandModel { get; set; } = null!;
        public string RegistrationNumber { get; set; } = null!;
        public FuelType FuelType { get; set; }
        public int? Mileage { get; set; }
        public string? VIN { get; set; }
        public int? Year { get; set; }
        public string? Details { get; set; }
        public bool IsActive { get; set; }
    }
}
