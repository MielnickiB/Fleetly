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

namespace FleetlyBackend.Services.OrderService
{
    public class OrderService(
        FleetlyContext context,
        IInvoiceService invoiceService,
        INotificationService notificationService,
        IExpenseService expenseService,
        ICostLimitService costLimitService,
        IHttpContextAccessor http
    ) : IOrderService
    {
        private readonly FleetlyContext _context = context;
        private readonly IInvoiceService _invoiceService = invoiceService;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IExpenseService _expenseService = expenseService;
        private readonly ICostLimitService _costLimitService = costLimitService;
        private readonly IHttpContextAccessor _http = http;

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
                if (order.WorkerId == userId) return order.ToResponseDto();

                if (order.Status == OrderStatus.Created && order.WorkerId == null)
                    return order.ToResponseDto();
            }

            throw new UnauthorizedAccessException("Brak dostępu do tego zlecenia.");
        }

        public async Task<PagedResult<OrderResponseDto>> GetAllOrders()
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

            var totalCount = await query.CountAsync();

            var orders = await query
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => o.ToResponseDto())
                .ToListAsync();

            return new PagedResult<OrderResponseDto>
            {
                Items = orders,
                TotalCount = totalCount
            };
        }

        public async Task<PagedResult<OrderResponseDto>> GetAvailableOrders()
        {
            var totalCount = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Status == OrderStatus.Created && o.WorkerId == null)
                .CountAsync();

            var orders = await _context.Orders
               .AsNoTracking()
               .IncludeAllOrderRelations()
               .Where(o => o.Status == OrderStatus.Created && o.WorkerId == null)
               .OrderByDescending(o => o.CreatedAt)
               .Select(o => o.ToResponseDto())
               .ToListAsync();

            return new PagedResult<OrderResponseDto>
            {
                Items = orders,
                TotalCount = totalCount
            };
        }

        #endregion

        #region CREATE + UPDATE + CANCEL

        public async Task<OrderResponseDto> CreateOrder(OrderCreateDto dto)
        {
            if (!await _context.Vehicles.AnyAsync(v => v.Id == dto.VehicleId))
                throw new ArgumentException("Pojazd nie istnieje.");

            if (dto.Type == OrderType.ServiceRide && (!dto.ServiceLocationId.HasValue || !dto.ServiceTime.HasValue))
                throw new InvalidOperationException("Zlecenie serwisowe wymaga lokalizacji serwisu i czasu dojazdu do niego.");

            ValidateOrderTime(dto.StartTime, dto.ServiceTime, dto.Deadline);
            await ValidateLocations(dto.StartLocationId, dto.ServiceLocationId, dto.EndLocationId);

            var user = _http.CurrentUser();
            var clientId = user.GetUserId();

            var costLimit = await _costLimitService.GetByRangeOfKm(dto.RangeOfKm)
                ?? throw new InvalidOperationException("Nie znaleziono limitu kosztów dla podanego dystansu.");

            var order = new Order
            {
                ClientId = clientId,
                VehicleId = dto.VehicleId,
                StartLocationId = dto.StartLocationId,
                ServiceLocationId = dto.ServiceLocationId,
                EndLocationId = dto.EndLocationId,
                Type = dto.Type,
                Details = dto.Details,
                RangeOfKm = dto.RangeOfKm,
                Salary = costLimit.BaseSalary,
                StartTime = dto.StartTime,
                ServiceTime = dto.ServiceTime,
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

            if (order.Type == OrderType.ServiceRide && (!dto.ServiceLocationId.HasValue || !dto.ServiceTime.HasValue))
                throw new ArgumentException("Zlecenie serwisowe musi posiadać lokalizację i czas serwisu.");

            var oldWorkerId = order.WorkerId;
            var workerChanged = false;

            if (dto.WorkerId != order.WorkerId)
            {
                if (order.Status >= OrderStatus.OrderStarted)
                    throw new InvalidOperationException("Nie można zmienić pracownika po rozpoczęciu zlecenia.");

                if (dto.WorkerId.HasValue)
                {
                    var isWorker = await _context.Users
                        .Include(u => u.Role)
                        .AnyAsync(u => u.Id == dto.WorkerId && u.Role.RoleName == "Worker");

                    if (!isWorker) throw new ArgumentException("Użytkownik nie jest pracownikiem.");

                    order.WorkerId = dto.WorkerId;
                    order.Status = OrderStatus.Assigned;
                    workerChanged = true;
                }
                else
                {
                    order.WorkerId = null;
                    order.Status = OrderStatus.Created;
                    workerChanged = true;
                }
            }

            if (dto.VehicleId != order.VehicleId)
            {
                if (!await _context.Vehicles.AnyAsync(v => v.Id == dto.VehicleId))
                    throw new ArgumentException("Pojazd nie istnieje.");
                order.VehicleId = dto.VehicleId;
            }

            bool locChanged =
                dto.StartLocationId != order.StartLocationId ||
                dto.EndLocationId != order.EndLocationId ||
                dto.ServiceLocationId != order.ServiceLocationId;

            if (locChanged)
            {
                await ValidateLocations(dto.StartLocationId, dto.ServiceLocationId, dto.EndLocationId);

                order.StartLocationId = dto.StartLocationId;
                order.EndLocationId = dto.EndLocationId;

                if (order.Type == OrderType.ServiceRide)
                    order.ServiceLocationId = dto.ServiceLocationId;
            }

            if (dto.RangeOfKm != order.RangeOfKm)
            {
                var costLimit = await _costLimitService.GetByRangeOfKm(dto.RangeOfKm)
                     ?? throw new InvalidOperationException("Brak limitu kosztów.");

                order.RangeOfKm = dto.RangeOfKm;
                order.Salary = costLimit.BaseSalary;
            }

            ValidateOrderTime(dto.StartTime, dto.ServiceTime, dto.Deadline);

            order.StartTime = dto.StartTime;
            order.Deadline = dto.Deadline;

            if (order.Type == OrderType.ServiceRide)
                order.ServiceTime = dto.ServiceTime;

            if (!string.IsNullOrEmpty(dto.EndContactName) && !dto.EndContactName.Equals(order.EndContactName))
                order.EndContactName = dto.EndContactName;

            if (!string.IsNullOrEmpty(dto.EndContactPhone) && !dto.EndContactPhone.Equals(order.EndContactPhone))
                order.EndContactPhone = dto.EndContactPhone;

            order.Details = dto.Details;

            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _notificationService.NotifyOrderUpdated(order, userId);

            if (workerChanged)
            {
                if (oldWorkerId.HasValue)
                    await _notificationService.NotifyWorkerUnassigned(order, oldWorkerId.Value);

                if (order.WorkerId.HasValue)
                    await _notificationService.NotifyWorkerAssigned(order, order.WorkerId.Value);
            }

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

        public async Task<OrderResponseDto> AcceptOrder(int orderId)
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

            return await GetFresh(orderId);
        }

        public async Task<OrderResponseDto> ResignOrder(int orderId)
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

            return await GetFresh(orderId);
        }

        public async Task<OrderResponseDto> StartOrder(int orderId)
        {
            var order = await GetOrderWithWorkerCheck(orderId);

            if (order.Status != OrderStatus.Assigned)
                throw new InvalidOperationException("Zlecenie nie jest gotowe do rozpoczęcia.");

            order.Status = OrderStatus.OrderStarted;
            order.ActualStartTime = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetFresh(orderId);
        }

        public async Task<OrderResponseDto> ArrivedToService(int orderId)
        {
            var order = await GetOrderWithWorkerCheck(orderId);

            if (order.Type != OrderType.ServiceRide)
                throw new InvalidOperationException("To zlecenie nie jest przejazdem do serwisu.");

            if (order.Status != OrderStatus.OrderStarted)
                throw new InvalidOperationException("Zlecenie nie jest rozpoczęte.");

            order.Status = OrderStatus.ArrivedAtServiceLocation;
            order.ActualArrivedServiceTime = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetFresh(orderId);
        }

        public async Task<OrderResponseDto> LeaveServiceLocation(int orderId)
        {
            var order = await GetOrderWithWorkerCheck(orderId);

            if (order.Type != OrderType.ServiceRide)
                throw new InvalidOperationException("To zlecenie nie jest przejazdem do serwisu.");

            if (order.Status != OrderStatus.ArrivedAtServiceLocation)
                throw new InvalidOperationException("Nie można opuścić serwisu — nie jesteś na miejscu.");

            order.Status = OrderStatus.LeftServiceLocation;
            order.ActualLeftServiceTime = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetFresh(orderId);
        }

        public async Task<OrderResponseDto> ArrivedToClient(int orderId)
        {
            var order = await GetOrderWithWorkerCheck(orderId);

            if (order.Type == OrderType.ServiceRide &&
                order.Status != OrderStatus.LeftServiceLocation)
                throw new InvalidOperationException("Musisz najpierw opuścić serwis.");
            else
                if (order.Status != OrderStatus.OrderStarted)
                throw new InvalidOperationException("Musisz wpierw wyjechać od klienta.");

            order.Status = OrderStatus.ArrivedToClient;
            order.ActualEndTime = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetFresh(orderId);
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

            if (order.Status != OrderStatus.OrderFinishedByWorker)
                throw new InvalidOperationException("Koszty można dodawać tylko po zakończeniu zlecenia.");

            var costLimit = await _costLimitService.GetByRangeOfKm(order.RangeOfKm)
                ?? throw new InvalidOperationException("Brak limitu kosztów.");

            if (!dto.IsFuelExpense && order.AdditionalCosts + dto.Cost > costLimit.MaxCosts)
                throw new InvalidOperationException($"Przekroczono limit kosztów dodatkowych ({costLimit.MaxCosts} PLN).");

            using var tx = await _context.Database.BeginTransactionAsync();

            var expense = await _expenseService.Create(orderId, dto);

            if (expense.IsFuelExpense)
                order.FuelCosts += expense.Cost;
            else
                order.AdditionalCosts += expense.Cost;

            order.UpdatedAt = DateTime.UtcNow;

            _context.Orders.Update(order);

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return await GetFresh(orderId);
        }

        public async Task<OrderResponseDto> UpdateCost(int orderId, int expenseId, ExpenseUpdateDto dto)
        {
            var order = await GetOrderWithWorkerCheck(orderId);

            if (order.Status != OrderStatus.OrderFinishedByWorker)
                throw new InvalidOperationException("Nie można modyfikować kosztów w tym stanie.");

            var expenseEntity = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.OrderId == orderId)
                ?? throw new ArgumentException("Koszt nie istnieje.");

            var oldCost = expenseEntity.Cost;
            var oldIsFuel = expenseEntity.IsFuelExpense;

            var costLimit = await _costLimitService.GetByRangeOfKm(order.RangeOfKm)
                ?? throw new InvalidOperationException("Brak limitu kosztów.");

            var newCostValue = dto.Cost ?? oldCost;
            var newIsFuel = dto.IsFuelExpense ?? oldIsFuel;

            if (!newIsFuel)
            {
                var currentAdditionalTotal = order.AdditionalCosts - (oldIsFuel ? 0 : oldCost);

                if (currentAdditionalTotal + newCostValue > costLimit.MaxCosts)
                    throw new InvalidOperationException($"Aktualizacja spowoduje przekroczenie limitu kosztów dodatkowych.");
            }

            using var tx = await _context.Database.BeginTransactionAsync();

            var updatedExpense = await _expenseService.Update(expenseId, dto);

            if (oldIsFuel) order.FuelCosts -= oldCost;
            else order.AdditionalCosts -= oldCost;

            if (updatedExpense.IsFuelExpense) order.FuelCosts += updatedExpense.Cost;
            else order.AdditionalCosts += updatedExpense.Cost;

            order.UpdatedAt = DateTime.UtcNow;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();

            return await GetFresh(orderId);
        }

        public async Task<OrderResponseDto> DeleteCost(int orderId, int expenseId)
        {
            var order = await GetOrderWithWorkerCheck(orderId);

            if (order.Status != OrderStatus.OrderFinishedByWorker)
                throw new InvalidOperationException("Nie można usuwać kosztów w tym stanie.");

            var expenseEntity = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.OrderId == orderId)
                ?? throw new ArgumentException("Koszt nie istnieje.");

            using var tx = await _context.Database.BeginTransactionAsync();

            if (expenseEntity.IsFuelExpense)
                order.FuelCosts -= expenseEntity.Cost;
            else
                order.AdditionalCosts -= expenseEntity.Cost;

            var deleted = await _expenseService.Delete(expenseId);
            if (!deleted)
                throw new InvalidOperationException("Nie udało się usunąć kosztu.");

            order.UpdatedAt = DateTime.UtcNow;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();

            return await GetFresh(orderId);
        }

        public async Task<OrderResponseDto> SubmitAllOrderCosts(int orderId)
        {
            var order = await GetOrderWithWorkerCheck(orderId);

            if (order.Status != OrderStatus.OrderFinishedByWorker)
                throw new InvalidOperationException("Zlecenie nie zostało zakończone przez pracownika.");

            order.Status = OrderStatus.WaitingForCostApproval;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _notificationService.NotifyOrderFinishedByWorker(order);

            return await GetFresh(orderId);
        }

        #endregion

        #region ADMIN

        public async Task<OrderResponseDto> ApproveCostsAndCompleteOrder(int orderId)
        {
            if (!_http.CurrentUser().IsAdmin())
                throw new UnauthorizedAccessException("Tylko administrator może zatwierdzać koszty.");

            using var tx = await _context.Database.BeginTransactionAsync();

            var order = await _context.Orders
                            .FirstOrDefaultAsync(o => o.Id == orderId)
                            ?? throw new ArgumentException("Zlecenie nie istnieje.");

            if (order.Status != OrderStatus.WaitingForCostApproval)
                throw new InvalidOperationException("Koszty nie mogą zostać zatwierdzone w bieżącym statusie zlecenia.");

            order.Status = OrderStatus.ApprovedByAdmin;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var invoiceSum = Math.Round((order.Salary * 0.3m) + order.FuelCosts + order.AdditionalCosts);

            var invoice = new InvoiceCreateDto
            {
                OrderId = order.Id,
                Sum = invoiceSum
            };

            await _invoiceService.Create(invoice);

            await tx.CommitAsync();

            await _notificationService.NotifyOrderApprovedByAdmin(order);
            return await GetFresh(orderId);
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

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new ArgumentException("Zlecenie nie istnieje.");

            if (order.WorkerId != workerId)
                throw new UnauthorizedAccessException("Nie jesteś przypisany do tego zlecenia.");

            return order;
        }

        private static void ValidateOrderTime(DateTime start, DateTime? service, DateTime deadline)
        {
            if (start < DateTime.UtcNow)
                throw new ArgumentException("Czas rozpoczęcia zlecenia nie może być w przeszłości.");

            if (deadline <= start)
                throw new ArgumentException("Czas zakończenia zlecenia musi być poźniejszy niż rozpoczęcia.");

            if (service.HasValue && (service.Value <= start || service.Value >= deadline))
                throw new ArgumentException("Czas serwisu musi być pomiędzy czasem rozpoczęcia a terminem.");
        }

        private async Task ValidateLocations(int startLocationId, int? serviceLocationId, int endLocationId)
        {
            if (!await _context.Locations.AnyAsync(l => l.Id == startLocationId))
                throw new ArgumentException("Podana lokalizacja początkowa nie istnieje.");

            if (serviceLocationId.HasValue && !await _context.Locations.AnyAsync(l => l.Id == serviceLocationId.Value))
                throw new ArgumentException("Podana lokalizacja serwisu nie istnieje.");

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
                .Include(o => o.Worker).ThenInclude(w => w.Details)
                .Include(o => o.Vehicle).ThenInclude(v => v.BrandModel).ThenInclude(bm => bm.CarBrand)
                .Include(o => o.StartLocation)
                .Include(o => o.ServiceLocation)
                .Include(o => o.EndLocation)
                .Include(o => o.Expenses);
        }
    }
}
