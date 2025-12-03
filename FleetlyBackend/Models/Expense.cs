namespace FleetlyBackend.Models
{
    public class Expense : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public decimal Cost { get; set; }
        public string CostPhotoUrl { get; set; } = null!;
        public bool IsFuelExpense { get; set; }
    }
}