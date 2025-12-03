using Fleetly.Shared.Dto.InvoiceDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Mappings
{
    public static class InvoiceMappings
    {
        public static InvoiceResponseDto ToResponseDto(this Invoice i)
            => new()
            {
                Id = i.Id,
                OrderId = i.OrderId,
                Sum = i.Sum,
                IsPaid = i.IsPaid,
                DateOfPayment = i.DateOfPayment,
                MethodOfPayment = i.MethodOfPayment
            };
    }
}
