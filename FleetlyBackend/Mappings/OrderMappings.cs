using Fleetly.Shared.Dto.OrderDtos;
using Fleetly.Shared.Dto.ExpenseDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Mappings
{
    public static class OrderMappings
    {
        public static OrderResponseDto ToResponseDto(this Order o)
            => new()
            {
                Id = o.Id,
                ClientId = o.ClientId,
                ClientName = o.Client.Details.Name + " " + o.Client.Details.Surname,
                WorkerId = o.WorkerId,
                WorkerName = o.Worker != null ? o.Worker.Details.Name + " " + o.Worker.Details.Surname : null,
                VehicleId = o.VehicleId,
                VehicleName = o.Vehicle.BrandModel.CarBrand.BrandName + " " + o.Vehicle.BrandModel.ModelName,
                VehicleRegistrationNumber = o.Vehicle.RegistrationNumber,
                Status = o.Status,
                Type = o.Type,
                Details = o.Details,

                StartLocationId = o.StartLocationId,
                StartLocationCity = o.StartLocation.City,
                StartLocationAddress = o.StartLocation.Street + " " + o.StartLocation.BuildingNumber + "/" + o.StartLocation.ApartmentNumber,
                ServiceLocationId = o.ServiceLocationId,
                ServiceLocationCity = o.ServiceLocation?.City,
                ServiceLocationAddress = o.ServiceLocation != null ? o.ServiceLocation.Street + " " + o.ServiceLocation.BuildingNumber + "/" + o.ServiceLocation.ApartmentNumber : null,
                EndLocationId = o.EndLocationId,
                EndLocationCity = o.EndLocation.City,
                EndLocationAddress = o.EndLocation.Street + " " + o.EndLocation.BuildingNumber + "/" + o.EndLocation.ApartmentNumber,
                RangeOfKm = o.RangeOfKm,

                Salary = o.Salary,
                AdditionalCosts = o.AdditionalCosts,
                FuelCosts = o.FuelCosts,
                Expenses = o.Expenses.Select(e => new ExpenseResponseDto
                {
                    Id = e.Id,
                    OrderId = e.OrderId,
                    Cost = e.Cost,
                    CostPhotoUrl = e.CostPhotoUrl,
                    IsFuelExpense = e.IsFuelExpense

                }).ToList(),

                StartTime = o.StartTime,
                ServiceTime = o.ServiceTime,
                Deadline = o.Deadline,

                IsActive = o.IsActive,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            };
    }
}
