namespace Fleetly.Shared.Dto.CostLimitDtos
{
    public class CostLimitUpdateDto
    {
        public int? RangeOfKmMin { get; set; }
        public int? RangeOfKmMax { get; set; }
        public decimal? BaseSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public decimal? MaxCosts { get; set; }
    }
}
