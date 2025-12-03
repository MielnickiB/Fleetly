using Fleetly.Shared.Dto.CostLimitDtos;
using FleetlyBackend.Models;

namespace FleetlyBackend.Mappings
{
    public static class CostLimitMappings
    {
        public static CostLimitResponseDto ToResponseDto(this CostLimit c)
            => new()
            {
                Id = c.Id,
                RangeOfKmMin = c.RangeOfKmMin,
                RangeOfKmMax = c.RangeOfKmMax,
                BaseSalary = c.BaseSalary,
                MaxSalary = c.MaxSalary,
                MaxCosts = c.MaxCosts
            };
    }
}
