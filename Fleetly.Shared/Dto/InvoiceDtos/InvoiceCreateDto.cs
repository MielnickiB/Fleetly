using System.ComponentModel.DataAnnotations;
using Fleetly.Shared.Enums;

namespace Fleetly.Shared.Dto.InvoiceDtos
{
    public class InvoiceCreateDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public decimal Sum { get; set; }
    }
}
