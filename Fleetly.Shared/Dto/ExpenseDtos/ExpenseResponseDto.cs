namespace Fleetly.Shared.Dto.ExpenseDtos
{
    public class ExpenseResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Cost { get; set; }
        public string CostPhotoUrl { get; set; } = null!;
        public bool IsFuelExpense { get; set; }
    }
}
