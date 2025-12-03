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
                WorkerId = o.WorkerId,
                VehicleId = o.VehicleId,
                Status = o.Status,
                Type = o.Type,
                Details = o.Details,

                StartLocationId = o.StartLocationId,
                ServiceLocationId = o.ServiceLocationId,
                EndLocationId = o.EndLocationId,
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
