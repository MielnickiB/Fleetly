using Fleetly.Shared.Enums;
using FleetlyBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Workers
{
    public class UrgentOrderWorker(IServiceScopeFactory scopeFactory, ILogger<UrgentOrderWorker> logger) : BackgroundService
    {
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);
        private readonly TimeSpan _initialInterval = TimeSpan.FromMinutes(5);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Aktualizacja cen: Startuje algorytm wyceny.");

            await Task.Delay(_initialInterval, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateUrgentOrdersAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Błąd podczas aktualizacji cen zleceń.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task UpdateUrgentOrdersAsync()
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FleetlyContext>();

            var now = DateTime.UtcNow;
            var warningThreshold = now.AddDays(2);
            var criticalThreshold = now.AddDays(1);

            var openOrders = await context.Orders
                .Include(o => o.CostLimit)
                .Where(o => o.WorkerId == null &&
                            o.Status == OrderStatus.Created &&
                            o.StartTime <= warningThreshold)
                .ToListAsync();

            foreach (var order in openOrders)
            {
                var oldSalary = order.Salary;
                var maxSalary = order.CostLimit?.MaxSalary ?? order.Salary;

                if (order.StartTime <= criticalThreshold)
                {
                    order.Salary = maxSalary;
                }
                else
                {
                    if (order.Salary >= maxSalary)
                        continue;

                    TimeSpan timeUntilCritical = order.StartTime - criticalThreshold;

                    double hoursLeft = Math.Max(1, timeUntilCritical.TotalHours);

                    decimal remainingMoney = maxSalary - order.Salary;

                    decimal bump = remainingMoney / (decimal)hoursLeft;

                    bump = Math.Round(bump, 2);

                    order.Salary = Math.Min(order.Salary + bump, maxSalary);
                }

                if (order.Salary != oldSalary)
                {
                    logger.LogInformation(
                        "Aktualizacja ceny zlecenia {OrderId}: {OldSalary} -> {NewSalary}",
                        order.Id, oldSalary, order.Salary);
                }
            }

            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync();
            }
        }
    }
}