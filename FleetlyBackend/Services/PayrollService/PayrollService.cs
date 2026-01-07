using FleetlyBackend.Data;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.PayrollService
{
    public class PayrollService(FleetlyContext context) : IPayrollService
    {
        private readonly FleetlyContext _context = context;

        public async Task AddAmountToPayrollAsync(int workerId, DateTime date, decimal amount)
        {
            var payrollMonth = new DateTime(date.Year, date.Month, 1);

            var payroll = await _context.Payrolls
                .FirstOrDefaultAsync(p => p.WorkerId == workerId && p.Date == payrollMonth);

            if (payroll == null)
            {
                payroll = new Payroll
                {
                    WorkerId = workerId,
                    Date = payrollMonth,
                    TotalAmount = 0,
                    IsPaid = false
                };
                await _context.Payrolls.AddAsync(payroll);
            }

            if (payroll.IsPaid)
            {
                throw new InvalidOperationException($"Miesiąc {payrollMonth:MM/yyyy} został już rozliczony. Nie można dodać kwoty.");
            }

            payroll.TotalAmount += amount;

            await _context.SaveChangesAsync();
        }
    }
}
