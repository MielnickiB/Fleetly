using Fleetly.Shared.Enums;

namespace Fleetly.Shared.Dto.InvoiceDtos
{
    public class InvoiceResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public string OrderTitle { get; set; } = string.Empty;
        public string VehicleName { get; set; } = string.Empty;

        public decimal Sum { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? DateOfPayment { get; set; }
        public DateTime DueDate { get; set; }
        public MethodOfPayment? MethodOfPayment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
