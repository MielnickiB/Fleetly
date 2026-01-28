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
                OrderTitle = $"Zlecenie #{i.OrderId}",
                VehicleName = $"{i.Order.Vehicle.BrandModel.CarBrand.BrandName} {i.Order.Vehicle.BrandModel.ModelName} - {i.Order.Vehicle.RegistrationNumber}",
                Sum = i.Sum,
                IsPaid = i.IsPaid,
                DateOfPayment = i.DateOfPayment,
                DueDate = i.DueDate,
                MethodOfPayment = i.MethodOfPayment,
                CreatedAt = i.CreatedAt
            };
    }
}
