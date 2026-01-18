namespace FleetlyBackend.Models
{
    public class Availability : BaseEntity
    {
        public int WorkerId { get; set; }
        public User Worker { get; set; } = null!;

        public DateOnly Date { get; set; }

        public TimeOnly StartHour { get; set; }
        public TimeOnly EndHour { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}