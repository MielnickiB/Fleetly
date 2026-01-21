using Fleetly.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.InvoiceDtos
{
    public class InvoicePayDto
    {
        [Required(ErrorMessage = "Metoda płatności jest wymagana.")]
        public MethodOfPayment MethodOfPayment { get; set; }
    }
}
