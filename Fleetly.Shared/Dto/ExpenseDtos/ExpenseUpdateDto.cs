using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Fleetly.Shared.Dto.ExpenseDtos
{
    public class ExpenseUpdateDto
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Koszt musi być większy od zera")]
        public decimal? Cost { get; set; }
        public bool? IsFuelExpense { get; set; }
        public IFormFile? CostPhoto { get; set; }
    }
}
