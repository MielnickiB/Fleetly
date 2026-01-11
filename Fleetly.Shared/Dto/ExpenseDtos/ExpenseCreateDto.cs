using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.ExpenseDtos
{
    public class ExpenseCreateDto
    {
        [Required(ErrorMessage = "Wartość kosztu jest wymagana!")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Koszt musi być większy od zera")]
        public decimal Cost { get; set; }
        [Required(ErrorMessage = "Typ kosztu jest wymagany!")]
        public bool IsFuelExpense { get; set; }
        [Required(ErrorMessage = "Zdjęcie kosztu jest wymagane!")]
        public IFormFile CostPhoto { get; set; } = null!;
    }
}
