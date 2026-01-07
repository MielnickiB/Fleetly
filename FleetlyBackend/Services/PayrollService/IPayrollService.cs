namespace FleetlyBackend.Services.PayrollService
{
    public interface IPayrollService
    {
        Task AddAmountToPayrollAsync(int workerId, DateTime date, decimal amount);
    }
}
