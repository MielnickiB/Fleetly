namespace Fleetly.Shared.Dto.DashboardDtos
{
    public class DashboardStatsDto
    {
        public int ActiveOrdersCount { get; set; }
        public int CompletedOrdersCount { get; set; }
        public decimal TotalRevenueMonth { get; set; }
        public int TotalClients { get; set; }
        public int TotalDrivers { get; set; }

        public double[] RevenueLast6Months { get; set; } = [];
        public string[] RevenueMonthsLabels { get; set; } = [];

        public double[] OrdersStatusData { get; set; } = [];
        public string[] OrdersStatusLabels { get; set; } = [];
    }
}