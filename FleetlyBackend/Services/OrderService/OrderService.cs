using FleetlyBackend.Data;
using FleetlyBackend.Helpers;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using FleetlyBackend.Services.UserSerivce;
using FleetlyBackend.Services.InvoiceService;
using FleetlyBackend.Services.NotificationService;
using FleetlyBackend.Services.ExpenseService;
using FleetlyBackend.Services.CostLimitService;
using Microsoft.EntityFrameworkCore;
using Fleetly.Shared.Dto.ExpenseDtos;
using Fleetly.Shared.Dto.OrderDtos;
using Fleetly.Shared.Dto.NotificationDtos;
using Fleetly.Shared.Dto.InvoiceDtos;
using Fleetly.Shared.Dto;
using Fleetly.Shared.Enums;

namespace FleetlyBackend.Services.OrderService
{
    public class OrderService(
        FleetlyContext context,
        IInvoiceService invoiceService,
        INotificationService notificationService,
        IExpenseService expenseService,
        ICostLimitService costLimitService,
        IUserService userService
    ) : IOrderService
    {
        private readonly FleetlyContext _context = context;
        private readonly IInvoiceService _invoiceService = invoiceService;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IExpenseService _expenseService = expenseService;
        private readonly ICostLimitService _costLimitService = costLimitService;
        private readonly IUserService _userService = userService;

        #region GET

        public async Task<OrderResponseDto?> GetOrderById(int orderId)
        {
            var order = await LoadFullOrder(orderId);
            return order?.ToResponseDto();
        }
        public async Task<OrderResponseDto?> GetOrderForClientById(int orderId, int clientId)
        {
            var order = await LoadFullOrder(orderId)
                ?? throw new ArgumentException("Zlecenie nie istnieje.");

            if (order.ClientId != clientId)
                throw new UnauthorizedAccessException("Brak dostępu do tego zlecenia.");

            return order.ToResponseDto();
        }

        public async Task<OrderResponseDto?> GetOrderForWorkerById(int orderId, int workerId)
        {
            var order = await LoadFullOrder(orderId)
                ?? throw new ArgumentException("Zlecenie nie istnieje.");
            if (order.Status > OrderStatus.Created && order.WorkerId != workerId)
                throw new UnauthorizedAccessException("Brak dostępu do tego zlecenia.");
            return order.ToResponseDto();
        }

        public async Task<PagedResult<OrderResponseDto>> GetAllOrders(int page, int pageSize)
        {
            var (skip, take) = PaginationHelper.Calculate(page, pageSize);

            var query = _context.Orders
                .IncludeAllOrderRelations()
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var orders = await query
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(o => o.ToResponseDto())
                .ToListAsync();

            return new PagedResult<OrderResponseDto>
            {
                Items = orders,
                TotalCount = totalCount
            };
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersForClient(int clientId, int page, int pageSize)
        {
            var (skip, take) = PaginationHelper.Calculate(page, pageSize);

            return await _context.Orders
                .AsNoTracking()
                .IncludeAllOrderRelations()
                .Where(o => o.ClientId == clientId)
                .OrderByDescending(o => o.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(o => o.ToResponseDto())
                .ToListAsync();
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersForWorker(int workerId, int page, int pageSize)
        {
            var (skip, take) = PaginationHelper.Calculate(page, pageSize);

            return await _context.Orders
                .AsNoTracking()
                .IncludeAllOrderRelations()
                .Where(o => o.WorkerId == workerId)
                .OrderByDescending(o => o.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(o => o.ToResponseDto())
                .ToListAsync();
        }

        public async Task<List<OrderResponseDto>> GetAvailableOrdersForWorker(int page, int pageSize)
        {
            var (skip, take) = PaginationHelper.Calculate(page, pageSize);
            return await _context.Orders
                .AsNoTracking()
                .IncludeAllOrderRelations()
                .Where(o => o.Status == OrderStatus.Created && o.WorkerId == null)
                .OrderByDescending(o => o.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(o => o.ToResponseDto())
                .ToListAsync();
        }

        #endregion

        #region CREATE + UPDATE + CANCEL

        public async Task<OrderResponseDto> CreateOrder(int clientId, OrderCreateDto dto)
        {
            if (!await _context.Users.AnyAsync(c => c.Id == clientId))
                throw new ArgumentException("Klient nie istnieje.");

            if (!await _context.Vehicles.AnyAsync(v => v.Id == dto.VehicleId))
                throw new ArgumentException("Pojazd nie istnieje.");

            if (dto.Type == OrderType.ServiceRide && (!dto.ServiceLocationId.HasValue || !dto.ServiceTime.HasValue))
                throw new InvalidOperationException("Zlecenie serwisowe wymaga lokalizacji serwisu i czasu dojazdu do niego.");

            ValidateOrderTime(dto.StartTime, dto.ServiceTime, dto.Deadline);
            ValidateLocations(dto.StartLocationId, dto.ServiceLocationId, dto.EndLocationId);

            var costLimit = await _costLimitService.GetByRangeOfKm(dto.RangeOfKm);

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
                Status = OrderStatus.PendingApproval
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await NotifyAdmins(NotificationType.OrderCreated,
                $"Utworzono zlecenie nr {order.Id}",
                $"Klient {clientId} utworzył nowe zlecenie.", order.Id);

            return order.ToResponseDto();
        }

        public async Task<OrderResponseDto> UpdateOrder(int orderId, OrderUpdateDto dto, int? userId)
        {

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new ArgumentException("Zlecenie nie istnieje.");

            if (userId.HasValue && order.ClientId != userId.Value)
                throw new UnauthorizedAccessException("Nie możesz edytować zlecenia innego klienta.");

            if (order.Status >= OrderStatus.OrderFinishedByWorker)
                throw new InvalidOperationException("Nie można edytować zakończonego zlecenia.");

            if (dto.VehicleId.HasValue)
            {
                if (!await _context.Vehicles.AnyAsync(v => v.Id == dto.VehicleId.Value))
                    throw new ArgumentException("Podany pojazd nie istnieje.");

                order.VehicleId = dto.VehicleId.Value;
            }

            if (dto.StartLocationId.HasValue && order.StartLocationId != dto.StartLocationId.Value)
            {
                if (!await _context.Locations.AnyAsync(l => l.Id == dto.StartLocationId))
                    throw new ArgumentException("Podana lokalizacja początkowa nie istnieje.");

                order.StartLocationId = dto.StartLocationId.Value;
            }

            if (order.Type == OrderType.ServiceRide && dto.ServiceLocationId.HasValue && order.ServiceLocationId != dto.ServiceLocationId.Value)
            {
                if (!await _context.Locations.AnyAsync(l => l.Id == dto.ServiceLocationId))
                    throw new ArgumentException("Podana lokalizacja serwisu nie istnieje.");

                order.ServiceLocationId = dto.ServiceLocationId.Value;
            }

            if (dto.EndLocationId.HasValue && order.EndLocationId != dto.EndLocationId.Value)
            {
                if (!await _context.Locations.AnyAsync(l => l.Id == dto.EndLocationId))
                    throw new ArgumentException("Podana lokalizacja końcowa nie istnieje.");

                order.StartLocationId = dto.EndLocationId.Value;
            }

            if (dto.RangeOfKm.HasValue && dto.RangeOfKm.Value > 0)
            {
                order.RangeOfKm = dto.RangeOfKm.Value;
                var costLimit = await _costLimitService.GetByRangeOfKm(order.RangeOfKm);
                order.Salary = costLimit.BaseSalary;
            }

            var newStart = dto.StartTime ?? order.StartTime;
            var newService = dto.ServiceTime ?? order.ServiceTime;
            var newDeadline = dto.Deadline ?? order.Deadline;

            ValidateOrderTime(newStart, newService, newDeadline);

            order.StartTime = newStart;
            if (order.Type == OrderType.ServiceRide)
                order.ServiceTime = newService;
            order.Deadline = newDeadline;

            if (dto.Details is not null)
                order.Details = dto.Details;

            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            if (userId.HasValue)
            {
                await NotifyAdmins(NotificationType.OrderUpdated,
                    $"Zaktualizowano zlecenie {order.Id}",
                    $"Użytkownik {userId} zaktualizował dane.", order.Id);
            }
            else
            {
                await NotifyClient(order.ClientId, NotificationType.OrderUpdated,
                    $"Twoje zlecenie nr {order.Id} zostało zaktualizowane",
                    $"Zmiany wprowadził admin.", order.Id);
            }

            return await GetFresh(orderId);
        }

        public async Task<OrderResponseDto> CancelOrder(int orderId, int? userId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new ArgumentException("Zlecenie nie istnieje.");

            if (userId.HasValue && order.ClientId != userId.Value)
                throw new UnauthorizedAccessException("Nie możesz anulować zlecenia innego klienta.");

            if (order.Status >= OrderStatus.OrderStarted)
                throw new InvalidOperationException("Nie można anulować rozpoczętego zlecenia.");

            await NotifyOrderCancelled(order);

            if (order.WorkerId.HasValue)
                order.WorkerId = null;
            order.Status = OrderStatus.Cancelled;
            order.IsActive = false;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return order.ToResponseDto();
        }

        #endregion

        #region WORKER ACTIONS

        public async Task<OrderResponseDto> AcceptOrder(int orderId, int workerId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new ArgumentException("Zlecenie nie istnieje.");

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == workerId)
                ?? throw new ArgumentException("Pracownik nie istnieje.");

            if (user.Role.RoleName != "Worker")
                throw new UnauthorizedAccessException("Użytkownik nie jest pracownikiem.");

            if (order.Status != OrderStatus.Created)
                throw new InvalidOperationException("Zlecenie nie jest dostępne do przyjęcia.");

            if (order.WorkerId.HasValue)
                throw new InvalidOperationException("Zlecenie zostało już przyjęte przez innego pracownika.");

            order.WorkerId = workerId;
            order.Status = OrderStatus.Assigned;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await NotifyOrderAccepted(order, workerId);

            return order.ToResponseDto();
        }

        public async Task<OrderResponseDto> ResignOrder(int orderId, int workerId)
        {
            var order = await WorkerMustOwnOrder(orderId, workerId);

            if (order.Status != OrderStatus.Assigned)
                throw new InvalidOperationException("Można zrezygnować tylko ze zlecenia w statusie 'Przypisane'.");

            order.WorkerId = null;
            order.Status = OrderStatus.Created;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await NotifyOrderResigned(order, workerId);

            return order.ToResponseDto();
        }

        public async Task<OrderResponseDto> StartOrder(int orderId, int workerId)
        {
            var order = await WorkerMustOwnOrder(orderId, workerId);

            if (order.Status != OrderStatus.Assigned)
                throw new InvalidOperationException("Zlecenie nie jest gotowe do rozpoczęcia.");

            order.Status = OrderStatus.OrderStarted;
            order.ActualStartTime = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return order.ToResponseDto();
        }

        public async Task<OrderResponseDto> ArrivedToService(int orderId, int workerId)
        {
            var order = await WorkerMustOwnOrder(orderId, workerId);

            if (order.Type != OrderType.ServiceRide)
                throw new InvalidOperationException("To zlecenie nie jest przejazdem do serwisu.");

            if (order.Status != OrderStatus.OrderStarted)
                throw new InvalidOperationException("Zlecenie nie jest rozpoczęte.");

            order.Status = OrderStatus.ArrivedAtServiceLocation;
            order.ActualArrivedServiceTime = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return order.ToResponseDto();
        }

        public async Task<OrderResponseDto> LeaveServiceLocation(int orderId, int workerId)
        {
            var order = await WorkerMustOwnOrder(orderId, workerId);

            if (order.Type != OrderType.ServiceRide)
                throw new InvalidOperationException("To zlecenie nie jest przejazdem do serwisu.");

            if (order.Status != OrderStatus.ArrivedAtServiceLocation)
                throw new InvalidOperationException("Nie można opuścić serwisu — nie jesteś na miejscu.");

            order.Status = OrderStatus.LeftServiceLocation;
            order.ActualLeftServiceTime = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return order.ToResponseDto();
        }

        public async Task<OrderResponseDto> ArrivedToClient(int orderId, int workerId)
        {
            var order = await WorkerMustOwnOrder(orderId, workerId);

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
            return order.ToResponseDto();
        }

        public async Task<OrderResponseDto> FinishOrderByWorker(int orderId, int workerId)
        {
            var order = await WorkerMustOwnOrder(orderId, workerId);

            if (order.Status != OrderStatus.ArrivedToClient)
                throw new InvalidOperationException("Zlecenie nie zostało wykonane.");

            order.Status = OrderStatus.OrderFinishedByWorker;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return order.ToResponseDto();
        }

        #endregion

        #region COSTS

        public async Task<OrderResponseDto> AddCost(int orderId, int workerId, ExpenseCreateDto dto)
        {
            var order = await WorkerMustOwnOrder(orderId, workerId);

            if (order.Status != OrderStatus.OrderFinishedByWorker)
                throw new InvalidOperationException("Koszty można dodawać tylko po zakończeniu zlecenia.");

            var costLimit = await _costLimitService.GetByRangeOfKm(order.RangeOfKm);

            if (!dto.IsFuelExpense && order.AdditionalCosts + dto.Cost > costLimit.MaxCosts)
                throw new InvalidOperationException("Przekroczono maksymalny limit kosztów dodatkowych dla tego zlecenia.");

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

        public async Task<OrderResponseDto> UpdateCost(int orderId, int workerId, int expenseId, ExpenseUpdateDto dto)
        {
            var order = await WorkerMustOwnOrder(orderId, workerId);

            if (order.Status != OrderStatus.OrderFinishedByWorker)
                throw new InvalidOperationException("Nie można modyfikować kosztów w tym stanie.");

            var expenseEntity = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.OrderId == orderId)
                ?? throw new ArgumentException("Koszt nie istnieje.");

            var oldCost = expenseEntity.Cost;
            var oldFuel = expenseEntity.IsFuelExpense;

            using var tx = await _context.Database.BeginTransactionAsync();

            var costLimit = await _costLimitService.GetByRangeOfKm(order.RangeOfKm);
            if (costLimit != null)
            {
                if (dto.IsFuelExpense.HasValue && !dto.IsFuelExpense.Value)
                {
                    var newCost = dto.Cost ?? oldCost;
                    if (order.AdditionalCosts - (oldFuel ? 0 : oldCost) + newCost > costLimit.MaxCosts)
                        throw new InvalidOperationException("Przekroczono maksymalny limit kosztów dodatkowych dla tego zlecenia.");
                }
                else if (!dto.IsFuelExpense.HasValue && !oldFuel && dto.Cost.HasValue)
                {
                    var newCost = dto.Cost.Value;
                    if (order.AdditionalCosts - oldCost + newCost > costLimit.MaxCosts)
                        throw new InvalidOperationException("Przekroczono maksymalny limit kosztów dodatkowych dla tego zlecenia.");
                }
            }

            var updated = await _expenseService.Update(expenseId, dto);

            if (oldFuel == updated.IsFuelExpense)
            {
                var delta = updated.Cost - oldCost;
                if (updated.IsFuelExpense) order.FuelCosts += delta;
                else order.AdditionalCosts += delta;
            }
            else
            {
                if (updated.IsFuelExpense)
                {
                    order.AdditionalCosts -= oldCost;
                    order.FuelCosts += updated.Cost;
                }
                else
                {
                    order.FuelCosts -= oldCost;
                    order.AdditionalCosts += updated.Cost;
                }
            }

            order.UpdatedAt = DateTime.UtcNow;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();

            return await GetFresh(orderId);
        }

        public async Task<OrderResponseDto> DeleteCost(int orderId, int workerId, int expenseId)
        {
            var order = await WorkerMustOwnOrder(orderId, workerId);

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

        public async Task<OrderResponseDto> SubmitAllOrderCosts(int orderId, int workerId)
        {
            var order = await WorkerMustOwnOrder(orderId, workerId);

            if (order.Status != OrderStatus.OrderFinishedByWorker)
                throw new InvalidOperationException("Zlecenie nie zostało zakończone przez pracownika.");

            order.Status = OrderStatus.WaitingForCostApproval;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await NotifyAdmins(NotificationType.OrderCompleted,
                $"Pracownik zakończył zlecenie nr {order.Id}",
                $"Koszty zostały zgłoszone i oczekują na akceptację.", order.Id);

            return order.ToResponseDto();
        }

        #endregion

        #region ADMIN

        public async Task<OrderResponseDto> ApproveCostsAndCompleteOrder(int orderId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            var order = await LoadFullOrder(orderId)
                ?? throw new ArgumentException("Zlecenie nie istnieje.");

            if (order.Status != OrderStatus.WaitingForCostApproval)
                throw new InvalidOperationException("Koszty nie zostały jeszcze zgłoszone.");

            order.Status = OrderStatus.ApprovedByAdmin;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var invoice = new InvoiceCreateDto
            {
                OrderId = order.Id,
                Sum = Math.Round((order.Salary * 0.3m) + order.FuelCosts + order.AdditionalCosts)
            };

            await _invoiceService.Create(invoice);

            await _context.SaveChangesAsync();

            await tx.CommitAsync();
            return order.ToResponseDto();
        }

        public async Task<OrderResponseDto> ApproveOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new ArgumentException("Dane zlecenie nie istnieje.");

            if (order.Status != OrderStatus.PendingApproval)
                throw new InvalidOperationException("Zlecenie jest już zatwierdzone lub w trakcie realizacji.");

            order.Status = OrderStatus.Created;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await NotifyClient(order.ClientId, NotificationType.OrderApprovedByAdmin,
                $"Twoje zlecenie nr {order.Id} zostało zatwierdzone",
                $"Zlecenie jest gotowe do przyjęcia przez pracowników.", order.Id);

            return order.ToResponseDto();
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
                ?? throw new InvalidOperationException("Zlecenie zniknęło po aktualizacji.");

            return fresh.ToResponseDto();
        }

        private async Task<Order> WorkerMustOwnOrder(int id, int workerId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id)
                ?? throw new ArgumentException("Zlecenie nie istnieje.");

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == workerId)
                ?? throw new ArgumentException("Pracownik nie istnieje.");

            if (user.Role.RoleName != "Worker")
                throw new InvalidOperationException("Użytkownik nie jest pracownikiem.");

            if (order.WorkerId != workerId)
                throw new UnauthorizedAccessException("Nie jesteś przypisany do zlecenia.");

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

        private async void ValidateLocations(int startLocationId, int? serviceLocationId, int endLocationId)
        {
            if (!await _context.Locations.AnyAsync(l => l.Id == startLocationId))
                throw new ArgumentException("Podana lokalizacja początkowa nie istnieje.");

            if (serviceLocationId.HasValue && !await _context.Locations.AnyAsync(l => l.Id == serviceLocationId.Value))
                throw new ArgumentException("Podana lokalizacja serwisu nie istnieje.");

            if (!await _context.Locations.AnyAsync(l => l.Id == endLocationId))
                throw new ArgumentException("Podana lokalizacja końcowa nie istnieje.");
        }

        private async Task NotifyAdmins(NotificationType type, string title, string msg, int orderId)
        {
            var admins = await _userService.GetAllByRole("Admin");
            var notifications = admins.Select(admin => new Notification { UserId = admin.Id, Type = type, Title = title, Message = msg, RelatedEntity = RelatedEntityType.Order, RelatedEntityId = orderId }).ToList();
            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }

        private async Task NotifyClient(int clientId, NotificationType type, string title, string msg, int orderId)
        {
            await _notificationService.Create(new NotificationCreateDto
            {
                UserId = clientId,
                Type = type,
                Title = title,
                Message = msg,
                RelatedEntityId = orderId,
                RelatedEntity = RelatedEntityType.Order
            });
        }

        private async Task NotifyWorker(int workerId, NotificationType type, string title, string msg, int orderId)
        {
            await _notificationService.Create(new NotificationCreateDto
            {
                UserId = workerId,
                Type = type,
                Title = title,
                Message = msg,
                RelatedEntityId = orderId,
                RelatedEntity = RelatedEntityType.Order
            });
        }

        private async Task NotifyOrderAccepted(Order order, int workerId)
        {
            await NotifyAdmins(NotificationType.WorkerTookTheOrder,
                "Akceptacja",
                $"Pracownik {workerId} zaakceptował zlecenie.", order.Id);

            await NotifyClient(order.ClientId, NotificationType.WorkerTookTheOrder,
                "Akceptacja",
                $"Zlecenie nr {order.Id} zostało przyjęte przez pracownika {workerId}.", order.Id);
        }

        private async Task NotifyOrderResigned(Order order, int workerId)
        {
            await NotifyAdmins(NotificationType.WorkerResignedFromOrder,
                "Rezygnacja",
                $"Pracownik {workerId} zrezygnował ze zlecenia.", order.Id);
            await NotifyClient(order.ClientId, NotificationType.WorkerResignedFromOrder,
                "Rezygnacja",
                $"Pracownik {workerId} zrezygnował ze zlecenia.", order.Id);
        }
        private async Task NotifyOrderCancelled(Order order)
        {
            await NotifyClient(order.ClientId, NotificationType.OrderCancelled,
                "Anulowanie zlecenia",
                $"Zlecenie nr {order.Id} zostało anulowane.", order.Id);

            if (order.WorkerId.HasValue)
            {
                await NotifyWorker(order.WorkerId.Value, NotificationType.OrderCancelled,
                    "Anulowanie zlecenia",
                    $"Zlecenie nr {order.Id} zostało anulowane.", order.Id);
            }
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
