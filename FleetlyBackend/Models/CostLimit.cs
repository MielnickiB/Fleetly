using System.ComponentModel.DataAnnotations.Schema;

namespace FleetlyBackend.Models
{
    public class CostLimit : BaseEntity
    {
        public int RangeOfKmMin { get; set; }
        public int RangeOfKmMax { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseSalary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxSalary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxCosts { get; set; }
    }
}