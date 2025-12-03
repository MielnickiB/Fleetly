using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetlyBackend.Models
{
    public class Payroll : BaseEntity
    {
        public int WorkerId { get; set; }
        public User Worker { get; set; } = null!;

        public DateTime Date { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; }
    }
}