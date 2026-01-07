using Fleetly.Shared.Dto.DashboardDtos;
using Fleetly.Shared.Enums;
using FleetlyBackend.Data;
using FleetlyBackend.Extensions;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;
using FleetlyBackend.Mappings;

namespace FleetlyBackend.Services.DashboardService
{
    public class DashboardService(FleetlyContext context, IHttpContextAccessor http) : IDashboardService
    {
        private readonly FleetlyContext _context = context;
        private readonly IHttpContextAccessor _http = http;

        public async Task<DashboardStatsDto> GetStatsAsync()
        {
            var stats = new DashboardStatsDto();
            var userId = _http.CurrentUser().GetUserId();
            var isAdmin = _http.CurrentUser().IsAdmin();

            var now = DateTime.UtcNow;

            var ordersQuery = _context.Orders.AsQueryable().AsNoTracking();

            if (!isAdmin)
            {
                ordersQuery = ordersQuery.Where(o => o.ClientId == userId);
            }

            stats.ActiveOrdersCount = await ordersQuery.CountAsync(o => o.Status < OrderStatus.WaitingForCostApproval && o.Status > OrderStatus.PendingApproval);

            stats.CompletedOrdersCount = await ordersQuery.CountAsync(o => o.Status == OrderStatus.ApprovedByAdmin);

            stats.TotalRevenueMonth = await ordersQuery
                .Where(o => o.Status == OrderStatus.ApprovedByAdmin && o.ActualEndTime.HasValue && o.ActualEndTime.Value.Month == DateTime.UtcNow.Month && o.ActualEndTime.Value.Year == DateTime.UtcNow.Year)
                .SumAsync(o => o.Salary * 0.3m);

            stats.TotalSavings = await ordersQuery
                .Where(o => o.Status == OrderStatus.ApprovedByAdmin)
                .SumAsync(o => (o.CostLimit.MaxSalary + o.CostLimit.MaxCosts) * 0.5m);

            if (!isAdmin)
                stats.TotalVehicles = await _context.Vehicles.AsNoTracking().CountAsync(v => v.UserId == userId);
            else
                stats.TotalVehicles = await _context.Vehicles.AsNoTracking().CountAsync();

            if (!isAdmin)
                stats.TotalLocations = await _context.Locations.AsNoTracking().CountAsync(l => l.UserId == userId);
            else
                stats.TotalLocations = await _context.Locations.AsNoTracking().CountAsync();

            stats.TotalOrders = await ordersQuery.CountAsync();

            if (isAdmin)
            {
                stats.TotalClients = await _context.Users.AsNoTracking().CountAsync(u => u.Role.RoleName.Equals("Client"));

                stats.TotalDrivers = await _context.Users.AsNoTracking().CountAsync(u => u.Role.RoleName.Equals("Worker"));
            }

            await PrepareHistoryChartData(stats, ordersQuery, now, isAdmin);

            await PrepareStatusChartData(stats, ordersQuery);

            return stats;
        }

        public async Task<DriverDashboardDto> GetDriverDashboardAsync()
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            var tomorrow = today.AddDays(1);
            var userId = _http.CurrentUser().GetUserId();
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var payroll = await _context.Payrolls
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.WorkerId == userId && p.Date == startOfMonth);

            var currentMonthSalary = payroll?.TotalAmount ?? 0m;

            var ordersQuery = _context.Orders
                .AsNoTracking()
                .Where(o => o.WorkerId == userId);

            var todayOrders = await ordersQuery
                .Where(o => o.StartTime.Date == today)
                .OrderBy(o => o.StartTime)
                .Include(o => o.Vehicle)
                .Include(o => o.StartLocation)
                .Include(o => o.EndLocation)
                .Select(o => o.ToLiteDto())
                .ToListAsync();

            var tomorrowOrders = await ordersQuery
                .Where(o => o.StartTime.Date == tomorrow)
                .OrderBy(o => o.StartTime)
                .Include(o => o.Vehicle)
                .Include(o => o.StartLocation)
                .Include(o => o.EndLocation)
                .Select(o => o.ToLiteDto())
                .ToListAsync();

            var completedOrdersCount = await ordersQuery
                .CountAsync(o => o.Status == OrderStatus.ApprovedByAdmin);

            return new DriverDashboardDto 
            {
                TodayOrders = todayOrders,
                TomorrowOrders = tomorrowOrders,
                CurrentMonthSalary = currentMonthSalary,
                CompletedOrdersCount = completedOrdersCount
            };
        }

        private static async Task PrepareHistoryChartData(DashboardStatsDto stats, IQueryable<Order> baseQuery, DateTime now, bool isAdmin)
        {
            var sixMonthsAgo = now.AddMonths(-5);

            var query = baseQuery
                .Where(x => x.Status == OrderStatus.ApprovedByAdmin && x.ActualEndTime >= new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1))
                .GroupBy(x => new { x.ActualEndTime!.Value.Year, x.ActualEndTime.Value.Month });

            var dbData = await query.Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Value = isAdmin ? g.Sum(x => x.Salary * 0.3m) : g.Count()
            })
            .ToListAsync();

            var dataPoints = new List<double>();
            var labels = new List<string>();

            for (int i = 0; i < 6; i++)
            {
                var currentLoopDate = sixMonthsAgo.AddMonths(i);

                var monthData = dbData.FirstOrDefault(d => d.Year == currentLoopDate.Year && d.Month == currentLoopDate.Month);

                dataPoints.Add(monthData != null ? (double)monthData.Value : 0);

                labels.Add(currentLoopDate.ToString("MMM", new System.Globalization.CultureInfo("pl-PL")));
            }

            stats.RevenueLast6Months = dataPoints.ToArray();
            stats.RevenueMonthsLabels = labels.ToArray();
        }
        private static async Task PrepareStatusChartData(DashboardStatsDto stats, IQueryable<Order> baseQuery)
        {
            var statusCounts = await baseQuery
                .GroupBy(x => x.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var orderStatuses = new[]
            {
                OrderStatus.PendingApproval,
                OrderStatus.Created,
                OrderStatus.Assigned,
                OrderStatus.OrderStarted,
                OrderStatus.ArrivedToClient,
                OrderStatus.OrderFinishedByWorker,
                OrderStatus.WaitingForCostApproval,
                OrderStatus.ApprovedByAdmin,
                OrderStatus.Cancelled
            };

            var data = new List<double>();
            var labels = new List<string>();

            foreach (var status in orderStatuses)
            {
                var count = statusCounts.FirstOrDefault(x => x.Status == status)?.Count ?? 0;
                if (count > 0)
                {
                    data.Add(count);
                    labels.Add(TranslateStatus(status));
                }
            }

            stats.OrdersStatusData = data.ToArray();
            stats.OrdersStatusLabels = labels.ToArray();
        }

        private static string TranslateStatus(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.PendingApproval => "Oczekuje na dodanie do systemu",
                OrderStatus.Created => "Dodane do systemu",
                OrderStatus.Assigned => "Przypisane do pracownika",
                OrderStatus.OrderStarted => "Zlecenie rozpoczęte",
                OrderStatus.ArrivedToClient => "Pracownik u klienta",
                OrderStatus.OrderFinishedByWorker => "Zlecenie zakończone przez pracownika",
                OrderStatus.WaitingForCostApproval => "Oczekuje na zatwierdzenie kosztów",
                OrderStatus.ApprovedByAdmin => "Zatwierdzone przez administratora",
                OrderStatus.Cancelled => "Anulowane",
                _ => status.ToString()
            };
        }
    }
}
