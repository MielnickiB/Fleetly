using FleetlyBackend.Data;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using FleetlyBackend.Services.InvoiceService;
using FleetlyBackend.Services.NotificationService;
using FleetlyBackend.Services.ExpenseService;
using FleetlyBackend.Services.CostLimitService;
using Microsoft.EntityFrameworkCore;
using Fleetly.Shared.Dto.ExpenseDtos;
using Fleetly.Shared.Dto.OrderDtos;
using Fleetly.Shared.Dto.InvoiceDtos;
using Fleetly.Shared.Dto;
using Fleetly.Shared.Enums;
using FleetlyBackend.Extensions;
using FleetlyBackend.Services.PayrollService;
using FleetlyBackend.Helpers;

namespace FleetlyBackend.Services.OrderService
{
    public class OrderService(
        FleetlyContext context,
        IInvoiceService invoiceService,
        INotificationService notificationService,
        IExpenseService expenseService,
        ICostLimitService costLimitService,
        IPayrollService payrollService,
        IHttpContextAccessor http
    ) : IOrderService
    {
        private readonly FleetlyContext _context = context;
        private readonly IInvoiceService _invoiceService = invoiceService;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IExpenseService _expenseService = expenseService;
        private readonly ICostLimitService _costLimitService = costLimitService;
        private readonly IPayrollService _payrollService = payrollService;
        private readonly IHttpContextAccessor _http = http;
        private const decimal ADMIN_INVOICE_OVERHEAD_PERCENTAGE = 0.3m;

        #region GET

        public async Task<OrderResponseDto?> GetOrderById(int orderId)
        {
            var user = _http.CurrentUser();
            var userId = user.GetUserId();

            var order = await LoadFullOrder(orderId);
            if (order is null) return null;

            if (user.IsAdmin())
                return order.ToResponseDto();

            if (user.IsClient() && order.ClientId == userId)
                return order.ToResponseDto();

            if (user.IsWorker())
            {
                if (order.WorkerId == userId)
                    return order.ToResponseDto();

                if (order.Status == OrderStatus.Created && order.WorkerId == null)
                    return order.ToResponseDto();
            }

            throw new UnauthorizedAccessException("Brak dostępu do tego zlecenia.");
        }

        public async Task<PagedResult<OrderResponseDto>> GetAllOrders(bool includeInactive = false)
        {
            var user = _http.CurrentUser();
            var userId = user.GetUserId();

            var query = _context.Orders
                .IncludeAllOrderRelations()
                .AsQueryable();

            if (user.IsClient())
                query = query.Where(o => o.ClientId == userId);

            else if (user.IsWorker())
                query = query.Where(o => o.WorkerId == userId);

            if (!includeInactive)
                query = query.Where(o => o.Status != OrderStatus.Cancelled);

            var totalCount = await query.CountAsync();

            var orders = await query
                .AsNoTracking()
                .OrderByDescending(o => o.StartTime)
                .Select(o => o.ToResponseDto())
                .ToListAsync();

            return new PagedResult<OrderResponseDto>
            {
                Items = orders,
                TotalCount = totalCount
            };
        }

        public async Task<PagedResult<OrderLiteDto>> GetAvailableOrders(
            int page = 1,
            int pageSize = 10,
            string? search = null,
            string? sortBy = null,
            bool descending = false)
        {
            var (skip, safe) = PaginationHelper.Calculate(page, pageSize);

            var query = _context.Orders
                .AsNoTracking()
                .Where(o => o.Status == OrderStatus.Created && o.WorkerId == null && o.StartTime.Date >= DateTime.UtcNow.Date);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var phrase = search.Trim().ToLower();
                query = query.Where(o =>
                        (o.StartLocation != null && o.StartLocation.City.ToLower().Contains(phrase)) ||

                        (o.EndLocation != null && o.EndLocation.City.ToLower().Contains(phrase)) ||

                        (o.Vehicle != null && o.Vehicle.BrandModel != null && o.Vehicle.BrandModel.CarBrand != null &&
                         o.Vehicle.BrandModel.CarBrand.BrandName.ToLower().Contains(phrase)) ||

                        (o.Vehicle != null && o.Vehicle.BrandModel != null &&
                         o.Vehicle.BrandModel.ModelName.ToLower().Contains(phrase))
                    );
            }

            query = sortBy?.ToLower() switch
            {
                "date" => descending
                ? query.OrderByDescending(o => o.StartTime)
                : query.OrderBy(o => o.StartTime),
                "salary" => descending
                ? query.OrderByDescending(o => o.Salary)
                : query.OrderBy(o => o.Salary),
                "city" => descending
                ? query.OrderByDescending(o => o.StartLocation.City)
                : query.OrderBy(o => o.StartLocation.City),

                _ => query.OrderByDescending(o => o.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var orders = await query
               .IncludeAllLiteOrderRelations()
               .Skip(skip)
               .Take(safe)
               .Select(o => o.ToLiteDto())
               .ToListAsync();

            return new PagedResult<OrderLiteDto>
            {
                Items = orders,
                TotalCount = totalCount,
                PageSize = safe,
                PageNumber = page
            };
        }

        #endregion

        #region CREATE + UPDATE + CANCEL

        public async Task<OrderResponseDto> CreateOrder(OrderCreateDto dto)
        {
            if (!await _context.Vehicles.AnyAsync(v => v.Id == dto.VehicleId))
                throw new ArgumentException("Pojazd nie istnieje.");

            ValidateOrderTime(dto.StartTime, dto.Deadline);
            await ValidateLocations(dto.StartLocationId, dto.EndLocationId);

            var user = _http.CurrentUser();
            var clientId = user.GetUserId();

            var costLimit = await _costLimitService.GetByRangeOfKm(dto.RangeOfKm)
                ?? throw new InvalidOperationException("Nie znaleziono limitu kosztów dla podanego dystansu.");

            var order = new Order
            {
                ClientId = clientId,
                VehicleId = dto.VehicleId,
                StartLocationId = dto.StartLocationId,
                EndLocationId = dto.EndLocationId,
                Details = dto.Details,
                RangeOfKm = dto.RangeOfKm,
                CostLimitId = costLimit.Id,
                Salary = costLimit.BaseSalary,
                StartTime = dto.StartTime,
                Deadline = dto.Deadline,
                EndContactName = dto.EndContactName,
                EndContactPhone = dto.EndContactPhone,
                Status = OrderStatus.PendingApproval
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await _context.Entry(order).Reference(o => o.StartLocation).LoadAsync();
            await _context.Entry(order).Reference(o => o.EndLocation).LoadAsync();

            await _notificationService.NotifyOrderCreated(order);

            return await GetFresh(order.Id);
        }

        public async Task<OrderResponseDto> UpdateOrder(int orderId, OrderUpdateDto dto)
        {
            var user = _http.CurrentUser();
            var userId = user.GetUserId();

            var order = await _context.Orders
                    .Include(o => o.Client)
                    .FirstOrDefaultAsync(o => o.Id == orderId)
                    ?? throw new ArgumentException("Zlecenie nie istnieje.");

            if (!user.IsAdmin() && order.ClientId != userId)
                throw new UnauthorizedAccessException("Nie masz uprawnień do edycji tego zlecenia.");

            if (order.Status >= OrderStatus.OrderFinishedByWorker)
                throw new InvalidOperationException("Nie można edytować zakończonego zlecenia.");

            if (dto.VehicleId != order.VehicleId)
            {
                if (!await _context.Vehicles.AnyAsync(v => v.Id == dto.VehicleId))
                    throw new ArgumentException("Pojazd nie istnieje.");
                order.VehicleId = dto.VehicleId;
            }

            bool locChanged =
                dto.StartLocationId != order.StartLocationId ||
                dto.EndLocationId != order.EndLocationId;

            if (locChanged)
            {
                await ValidateLocations(dto.StartLocationId, dto.EndLocationId);

                order.StartLocationId = dto.StartLocationId;
                order.EndLocationId = dto.EndLocationId;
            }

            if (dto.RangeOfKm != order.RangeOfKm)
            {
                var costLimit = await _costLimitService.GetByRangeOfKm(dto.RangeOfKm)
                     ?? throw new InvalidOperationException("Brak limitu kosztów.");

                if (costLimit.Id != order.CostLimitId)
                {
                    order.CostLimitId = costLimit.Id;
                    order.Salary = costLimit.BaseSalary;
                }
                order.RangeOfKm = dto.RangeOfKm;
            }

            ValidateOrderTime(dto.StartTime, dto.Deadline);

            order.StartTime = dto.StartTime;
            order.Deadline = dto.Deadline;

            if (!string.IsNullOrEmpty(dto.EndContactName) && !dto.EndContactName.Equals(order.EndContactName))
                order.EndContactName = dto.EndContactName;

            if (!string.IsNullOrEmpty(dto.EndContactPhone) && !dto.EndContactPhone.Equals(order.EndContactPhone))
                order.EndContactPhone = dto.EndContactPhone;

            order.Details = dto.Details;

            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _notificationService.NotifyOrderUpdated(order, userId);

            return await GetFresh(orderId);
        }

        public async Task<OrderResponseDto> CancelOrder(int orderId)
        {
            var user = _http.CurrentUser();
            var currentUserId = user.GetUserId();

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new ArgumentException("Zlecenie nie istnieje.");

            if (!user.IsAdmin() && order.ClientId != currentUserId)
                throw new UnauthorizedAccessException("Nie masz uprawnień do anulowania tego zlecenia.");

            if (order.Status >= OrderStatus.OrderStarted)
                throw new InvalidOperationException("Nie można anulować rozpoczętego zlecenia.");

            order.WorkerId = null;
            order.Status = OrderStatus.Cancelled;
            order.IsActive = false;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _notificationService.NotifyOrderCancelled(order, currentUserId);

            return await GetFresh(orderId);
        }

        #endregion

        #region WORKER ACTIONS

        public async Task AcceptOrder(int orderId)
        {
            var user = _http.CurrentUser();

            if (!user.IsWorker())
                throw new UnauthorizedAccessException("Tylko pracownik może przyjąć zlecenie.");

            var workerId = user.GetUserId();

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new ArgumentException("Zlecenie nie istnieje.");

            if (order.Status != OrderStatus.Created)
                throw new InvalidOperationException("Zlecenie nie jest dostępne do przyjęcia.");

            if (order.WorkerId.HasValue)
                throw new InvalidOperationException("Zlecenie zostało już przyjęte przez innego pracownika.");

            order.WorkerId = workerId;
            order.Status = OrderStatus.Assigned;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _notificationService.NotifyWorkerAccepted(order);
        }

        public async Task ResignOrder(int orderId)
        {
            var order = await GetOrderWithWorkerCheck(orderId);

            if (order.Status != OrderStatus.Assigned)
                throw new InvalidOperationException("Można zrezygnować tylko ze zlecenia w statusie 'Przypisane'.");

            if (order.StartTime <= DateTime.UtcNow.AddDays(1))
                throw new InvalidOperationException("Nie można zrezygnować ze zlecenia na mniej niż dzień przed jego rozpoczęciem.");

            order.WorkerId = null;
            order.Status = OrderStatus.Created;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _notificationService.NotifyWorkerResigned(order);
        }

        public async Task<OrderResponseDto> FinishOrderByWorker(int orderId)
        {
            var order = await GetOrderWithWorkerCheck(orderId);

            if (order.Status != OrderStatus.ArrivedToClient)
                throw new InvalidOperationException("Zlecenie nie zostało wykonane.");

            order.Status = OrderStatus.OrderFinishedByWorker;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetFresh(orderId);
        }

        #endregion

        #region COSTS

        public async Task<OrderResponseDto> AddCost(int orderId, ExpenseCreateDto dto)
        {
            var order = await GetOrderWithWorkerCheck(orderId);

            if (order.Status < OrderStatus.Assigned || order.Status > OrderStatus.WaitingForCostApproval)
            {
                throw new InvalidOperationException("Koszty można dodawać tylko do zleceń przypisanych i nie zakończonych przez pracownika.");
            }

            if (!dto.IsFuelExpense && order.AdditionalCosts + dto.Cost > order.CostLimit.MaxCosts)
                throw new InvalidOperationException($"Przekroczono limit kosztów dodatkowych ({order.CostLimit.MaxCosts} PLN).");

            var expenseResponse = await _expenseService.Create(orderId, dto);

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {

                if (dto.IsFuelExpense)
                    order.FuelCosts += dto.Cost;
                else
                    order.AdditionalCosts += dto.Cost;

                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                return await GetFresh(orderId);
            }
            catch
            {
                await tx.RollbackAsync();
                await _expenseService.Delete(expenseResponse.Id);
                throw;
            }
        }

        public async Task<OrderResponseDto> UpdateCost(int orderId, int expenseId, ExpenseUpdateDto dto)
        {
            var order = await GetOrderWithWorkerCheck(orderId);

            if (order.Status < OrderStatus.Assigned || order.Status > OrderStatus.WaitingForCostApproval)
            {
                throw new InvalidOperationException("Nie można modyfikować kosztów w tym stanie.");
            }

            var oldExpense = await _context.Expenses.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.OrderId == orderId)
                ?? throw new ArgumentException("Koszt nie istnieje.");

            var oldCost = oldExpense.Cost;
            var oldIsFuel = oldExpense.IsFuelExpense;

            var newCostValue = dto.Cost ?? oldCost;
            var newIsFuel = dto.IsFuelExpense ?? oldIsFuel;

            if (!newIsFuel)
            {
                var currentAdditionalTotal = order.AdditionalCosts - (oldIsFuel ? 0 : oldCost);

                if (currentAdditionalTotal + newCostValue > order.CostLimit.MaxCosts)
                    throw new InvalidOperationException($"Aktualizacja spowoduje przekroczenie limitu kosztów dodatkowych.");
            }

            var updatedExpense = await _expenseService.Update(expenseId, dto);

            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                if (oldIsFuel) order.FuelCosts -= oldCost;
                else order.AdditionalCosts -= oldCost;

                if (updatedExpense.IsFuelExpense) order.FuelCosts += updatedExpense.Cost;
                else order.AdditionalCosts += updatedExpense.Cost;

                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                return await GetFresh(orderId);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<OrderResponseDto> DeleteCost(int orderId, int expenseId)
        {
            var order = await GetOrderWithWorkerCheck(orderId);

            if (order.Status < OrderStatus.Assigned || order.Status > OrderStatus.WaitingForCostApproval)
            {
                throw new InvalidOperationException("Nie można usuwać kosztów w tym stanie zlecenia.");
            }

            var expenseToDelete = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.OrderId == orderId)
                ?? throw new ArgumentException("Koszt nie istnieje.");

            var deleted = await _expenseService.Delete(expenseId);
            if (!deleted)
                throw new InvalidOperationException("Nie udało się usunąć kosztu.");

            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                if (expenseToDelete.IsFuelExpense)
                    order.FuelCosts -= expenseToDelete.Cost;
                else
                    order.AdditionalCosts -= expenseToDelete.Cost;

                order.UpdatedAt = DateTime.UtcNow;

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                return await GetFresh(orderId);
            }
            catch
            {
                await tx.RollbackAsync();
                await _expenseService.Create(orderId, new ExpenseCreateDto
                {
                    Cost = expenseToDelete.Cost,
                    IsFuelExpense = expenseToDelete.IsFuelExpense
                });
                throw;
            }
        }

        public async Task SubmitAllOrderCosts(int orderId)
        {
            var order = await GetOrderWithWorkerCheck(orderId);

            if (order.Status != OrderStatus.OrderFinishedByWorker)
                throw new InvalidOperationException("Zlecenie nie zostało zakończone przez pracownika.");

            order.Status = OrderStatus.WaitingForCostApproval;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _notificationService.NotifyOrderFinishedByWorker(order);
        }

        #endregion

        #region ADMIN

        public async Task ApproveCostsAndCompleteOrder(int orderId)
        {
            if (!_http.CurrentUser().IsAdmin())
                throw new UnauthorizedAccessException("Tylko administrator może zatwierdzać koszty.");

            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Orders
                                .FirstOrDefaultAsync(o => o.Id == orderId)
                                ?? throw new ArgumentException("Zlecenie nie istnieje.");

                if (order.Status != OrderStatus.WaitingForCostApproval)
                    throw new InvalidOperationException("Koszty nie mogą zostać zatwierdzone w bieżącym statusie zlecenia.");

                order.Status = OrderStatus.ApprovedByAdmin;
                order.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var invoiceSum = Math.Round(order.Salary + (order.Salary * ADMIN_INVOICE_OVERHEAD_PERCENTAGE) + order.FuelCosts + order.AdditionalCosts);

                var invoice = new InvoiceCreateDto
                {
                    OrderId = order.Id,
                    Sum = invoiceSum,
                    MethodOfPayment = null
                };

                await _invoiceService.Create(invoice);

                await _payrollService.AddAmountToPayrollAsync(order.WorkerId!.Value, order.StartTime, order.Salary);

                await tx.CommitAsync();

                await _notificationService.NotifyOrderApprovedByAdmin(order);
            }
            catch (Exception)
            {
                await tx.RollbackAsync();
                throw new Exception("Nie udało się zakceptować realizacja zlecenia.");
            }
        }

        public async Task<OrderResponseDto> ApproveOrder(int orderId)
        {
            if (!_http.CurrentUser().IsAdmin())
                throw new UnauthorizedAccessException("Tylko administrator może akceptować zlecenia.");

            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new ArgumentException("Dane zlecenie nie istnieje.");

            if (order.Status != OrderStatus.PendingApproval)
                throw new InvalidOperationException("Zlecenie jest już zatwierdzone lub w trakcie realizacji.");

            order.Status = OrderStatus.Created;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _notificationService.NotifyOrderActivated(order);

            return await GetFresh(orderId);
        }

        #endregion

        #region HELPERS

        private async Task<Order?> LoadFullOrder(int id)
        {
            return await _context.Orders
                .AsNoTracking()
                .IncludeAllOrderRelations()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        private async Task<OrderResponseDto> GetFresh(int orderId)
        {
            var fresh = await LoadFullOrder(orderId)
                ?? throw new InvalidOperationException("Wystąpił problem ze znalezeniem zlecenia.");

            return fresh.ToResponseDto();
        }

        private async Task<Order> GetOrderWithWorkerCheck(int orderId)
        {
            var user = _http.CurrentUser();
            if (!user.IsWorker())
                throw new UnauthorizedAccessException("Tylko pracownik może wykonać tę akcję.");

            var workerId = user.GetUserId();

            var order = await _context.Orders
                .IncludeAllLiteOrderRelations()
                .FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new ArgumentException("Zlecenie nie istnieje.");

            if (order.WorkerId != workerId)
                throw new UnauthorizedAccessException("Nie jesteś przypisany do tego zlecenia.");

            return order;
        }

        private static void ValidateOrderTime(DateTime start, DateTime deadline)
        {
            if (start < DateTime.UtcNow)
                throw new ArgumentException("Czas rozpoczęcia zlecenia nie może być w przeszłości.");

            if (deadline <= start)
                throw new ArgumentException("Czas zakończenia zlecenia musi być poźniejszy niż rozpoczęcia.");
        }

        private async Task ValidateLocations(int startLocationId, int endLocationId)
        {
            if (!await _context.Locations.AnyAsync(l => l.Id == startLocationId))
                throw new ArgumentException("Podana lokalizacja początkowa nie istnieje.");

            if (!await _context.Locations.AnyAsync(l => l.Id == endLocationId))
                throw new ArgumentException("Podana lokalizacja końcowa nie istnieje.");
        }

        #endregion
    }

    public static class OrderIncludeExtensions
    {
        public static IQueryable<Order> IncludeAllOrderRelations(this IQueryable<Order> query)
        {
            return query
                .Include(o => o.Client).ThenInclude(c => c.Details)
                .Include(o => o.Client).ThenInclude(c => c.Role)
                .Include(o => o.Worker).ThenInclude(w => w.Details)
                .Include(o => o.Worker).ThenInclude(w => w.Role)
                .Include(o => o.Vehicle).ThenInclude(v => v.BrandModel).ThenInclude(bm => bm.CarBrand)
                .Include(o => o.Vehicle).ThenInclude(v => v.User).ThenInclude(u => u.Details)
                .Include(o => o.CostLimit)
                .Include(o => o.StartLocation).ThenInclude(l => l.User).ThenInclude(u => u.Details)
                .Include(o => o.EndLocation).ThenInclude(l => l.User).ThenInclude(u => u.Details)
                .Include(o => o.Expenses);
        }

        public static IQueryable<Order> IncludeAllLiteOrderRelations(this IQueryable<Order> query)
        {
            return query
                .Include(o => o.Client).ThenInclude(c => c.Details)
                .Include(o => o.Worker).ThenInclude(w => w.Details)
                .Include(o => o.Vehicle).ThenInclude(v => v.BrandModel).ThenInclude(bm => bm.CarBrand)
                .Include(o => o.CostLimit)
                .Include(o => o.StartLocation)
                .Include(o => o.EndLocation);
        }
    }
}
