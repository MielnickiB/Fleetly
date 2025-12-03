using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.ExpenseDtos
{
    public class ExpenseCreateDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Koszt musi być większy od zera")]
        public decimal Cost { get; set; }
        [Required]
        public bool IsFuelExpense { get; set; }
        [Required]
        public IFormFile CostPhoto { get; set; } = null!;
    }
}
