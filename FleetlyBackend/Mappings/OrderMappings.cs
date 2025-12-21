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
                ClientCompanyName = o.Client.Details.Company ?? "Nie dotyczy",
                ClientPhoneNumber = o.Client.Details.PhoneNumber,
                ClientEmail = o.Client.Email,
                WorkerId = o.WorkerId,
                WorkerName = o.Worker != null ? o.Worker.Details.Name + " " + o.Worker.Details.Surname : "Nie przypisano",
                VehicleId = o.VehicleId,
                VehicleName = o.Vehicle.BrandModel.CarBrand.BrandName + " " + o.Vehicle.BrandModel.ModelName,
                VehicleRegistrationNumber = o.Vehicle.RegistrationNumber,
                VehicleMileage = o.Vehicle.Mileage,
                VehicleVin = o.Vehicle.VIN ?? "Nie podano",
                Status = o.Status,
                Type = o.Type,
                Details = o.Details,

                StartLocationId = o.StartLocationId,
                StartLocationCity = o.StartLocation.City,
                StartLocationAddress = FormatAddress(o.StartLocation.Street, o.StartLocation.BuildingNumber, o.StartLocation.ApartmentNumber),
                ServiceLocationId = o.ServiceLocationId,
                ServiceLocationCity = o.ServiceLocation?.City,
                ServiceLocationAddress = o.ServiceLocation != null
                    ? FormatAddress(o.ServiceLocation.Street, o.ServiceLocation.BuildingNumber, o.ServiceLocation.ApartmentNumber)
                    : null,
                EndLocationId = o.EndLocationId,
                EndLocationCity = o.EndLocation.City,
                EndLocationAddress = FormatAddress(o.EndLocation.Street, o.EndLocation.BuildingNumber, o.EndLocation.ApartmentNumber),
                RangeOfKm = o.RangeOfKm,

                EndContactName = o.EndContactName,
                EndContactPhone = o.EndContactPhone,

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

        private static string FormatAddress(string street, string buildingNumber, string? apartmentNumber)
        {
            var apt = string.IsNullOrWhiteSpace(apartmentNumber) ? string.Empty : "/" + apartmentNumber.Trim();
            return $"{street} {buildingNumber}{apt}".Trim();
        }
    }
}