using Fleetly.Shared.Enums;

namespace Fleetly.Shared.Dto.InvoiceDtos
{
    public class InvoiceUpdateDto
    {
        public decimal? Sum { get; set; }
        public bool? IsPaid { get; set; }
        public MethodOfPayment? MethodOfPayment { get; set; }
    }
}
