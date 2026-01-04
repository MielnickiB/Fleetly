using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.CostLimitDtos
{
    public class CostLimitBaseDto
    {
        [Required(ErrorMessage ="Minimalny dystans jest wymagany!")]
        [Range(0, int.MaxValue, ErrorMessage = "Minimalny dystans nie może być mniejszy niż 0!")]
        public int RangeOfKmMin { get; set; }
        [Required(ErrorMessage = "Maksymalny dystans jest wymagany!")]
        [Range(0, int.MaxValue, ErrorMessage = "Maksymalny dystans nie może być mniejszy niż 0!")]
        public int RangeOfKmMax { get; set; }
        [Required(ErrorMessage = "Podstawowe wynagrodzenie jest wymagane!")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Podstawowe wynagrodzenie musi być większe niż 0!")]
        public decimal BaseSalary { get; set; }
        [Required(ErrorMessage = "Maksymalne wynagrodzenie jest wymagane!")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Maksymalne wynagrodzenie musi być większe niż 0!")]
        public decimal MaxSalary { get; set; }
        [Required(ErrorMessage = "Limit kosztów jest wymagany!")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Limit kosztów musi być większe niż 0!")]
        public decimal MaxCosts { get; set; }
        public bool IsActive { get; set; }
    }
}
