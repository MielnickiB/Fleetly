using Fleetly.Shared.Enums;

namespace Fleetly.Shared.Dto.VehicleDtos
{
    public class VehicleFilterQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }

        public string? SearchTerm { get; set; }

        public string? UserFullName { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? BrandName { get; set; }
        public string? ModelName { get; set; }
        public FuelType? FuelType { get; set; }
        public bool? IsActive { get; set; }
    }
}
