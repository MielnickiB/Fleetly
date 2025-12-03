namespace Fleetly.Shared.Dto.CostLimitDtos
{
    public class CostLimitCreateDto
    {
        public int RangeOfKmMin { get; set; }
        public int RangeOfKmMax { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public decimal MaxCosts { get; set; }
    }
}
