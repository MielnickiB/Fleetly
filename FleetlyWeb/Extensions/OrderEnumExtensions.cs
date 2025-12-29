using Fleetly.Shared.Enums;
using MudBlazor;

namespace FleetlyWeb.Extensions
{
    public static class OrderEnumExtensions
    {
        public static string ToPolishName(this OrderStatus status) => status switch
        {
            OrderStatus.Created => "Utworzone",
            OrderStatus.PendingApproval => "Oczekuje na akceptację",
            OrderStatus.Assigned => "Przypisane",
            OrderStatus.OrderStarted => "W trasie",
            OrderStatus.ArrivedAtServiceLocation => "W serwisie",
            OrderStatus.LeftServiceLocation => "Wyjazd z serwisu",
            OrderStatus.ArrivedToClient => "U klienta",
            OrderStatus.OrderFinishedByWorker => "Zakończone (Czeka na koszty)",
            OrderStatus.WaitingForCostApproval => "Weryfikacja kosztów",
            OrderStatus.ApprovedByAdmin => "Zatwierdzone (Faktura)",
            OrderStatus.Cancelled => "Anulowane",
            _ => status.ToString()
        };

        public static Color ToColor(this OrderStatus status) => status switch
        {
            OrderStatus.Created => Color.Info,
            OrderStatus.PendingApproval => Color.Warning,
            OrderStatus.Assigned => Color.Primary,
            OrderStatus.OrderStarted => Color.Secondary,
            OrderStatus.ApprovedByAdmin => Color.Success,
            OrderStatus.Cancelled => Color.Error,
            _ => Color.Default
        };

        public static string ToPolishName(this OrderType type) => type switch
        {
            OrderType.CountryRide => "Krajowy",
            OrderType.ServiceRide => "Serwisowy",
            _ => type.ToString()
        };
    }
}