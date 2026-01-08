using Fleetly.Shared.Dto.OrderDtos;

namespace Fleetly.Shared.Dto.DashboardDtos
{
    public class DriverDashboardDto
    {
        public List<OrderLiteDto> TodayOrders { get; set; } = [];
        public List<OrderLiteDto> UpcomingOrders { get; set; } = [];

        public decimal CurrentMonthSalary { get; set; }

        public int CompletedOrdersCount { get; set; }
    }
}
