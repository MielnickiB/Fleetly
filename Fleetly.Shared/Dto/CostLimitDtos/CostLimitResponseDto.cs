namespace Fleetly.Shared.Dto.CostLimitDtos
{
    public class CostLimitResponseDto
    {
        public int Id { get; set; }
        public int RangeOfKmMin { get; set; }
        public int RangeOfKmMax { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public decimal MaxCosts { get; set; }
        public bool IsActive { get; set; }
    }
}
