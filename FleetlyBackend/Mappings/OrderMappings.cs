using Fleetly.Shared.Dto.OrderDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Mappings
{
    public static class OrderMappings
    {
        public static OrderResponseDto ToResponseDto(this Order o)
            => new()
            {
                Id = o.Id,

                Client = o.Client.ToResponseDto(),
                Worker = o.Worker?.ToResponseDto(),
                Vehicle = o.Vehicle.ToResponseDto(),

                StartLocation = o.StartLocation.ToLocationResponseDto(),
                EndLocation = o.EndLocation.ToLocationResponseDto(),

                Status = o.Status,
                Details = o.Details,

                RangeOfKm = o.RangeOfKm,
                Salary = o.Salary,
                AdditionalCosts = o.AdditionalCosts,
                FuelCosts = o.FuelCosts,

                MaxNonFuelCosts = o.CostLimit.MaxCosts,
                MaxSalary = o.CostLimit.MaxSalary,

                Expenses = o.Expenses?.Select(e => e.ToResponseDto()).ToList() ?? [],
                Protocols = o.Protocols?.Select(p => p.ToLiteDto()).ToList() ?? [],

                EndContactName = o.EndContactName,
                EndContactPhone = o.EndContactPhone,

                StartTime = o.StartTime,
                Deadline = o.Deadline,

                ActualStartTime = o.ActualStartTime,
                ActualEndTime = o.ActualEndTime,

                IsActive = o.IsActive,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            };

        public static OrderLiteDto ToLiteDto(this Order o)
            => new()
            {
                Id = o.Id,
                Car = $"{o.Vehicle.BrandModel.CarBrand.BrandName} {o.Vehicle.BrandModel.ModelName}",
                RegistrationNumber = o.Vehicle.RegistrationNumber,
                StartCity = o.StartLocation.City,
                EndCity = o.EndLocation.City,
                Salary = o.Salary,
                StartTime = o.StartTime,
                Status = o.Status
            };
    }
}