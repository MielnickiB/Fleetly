using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.InvoiceDtos
{
    public class InvoiceCreateDto
    {
        [Required(ErrorMessage = "Zlecenie jest wymagane do utworzenia faktury")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Suma do zapłaty jest wymagana!")]
        public decimal Sum { get; set; }
    }
}
