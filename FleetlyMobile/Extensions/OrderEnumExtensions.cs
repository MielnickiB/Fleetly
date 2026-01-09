using Fleetly.Shared.Enums;

namespace FleetlyMobile.Extensions
{
    public static class OrderEnumExtensions
    {
        public static string ToPolishName(this OrderStatus status) => status switch
        {
            OrderStatus.Created => "Utworzone",
            OrderStatus.PendingApproval => "Oczekuje na akceptację",
            OrderStatus.Assigned => "Przypisane",
            OrderStatus.OrderStarted => "W trasie",
            OrderStatus.ArrivedToClient => "U klienta",
            OrderStatus.OrderFinishedByWorker => "Zakończone (Czeka na koszty)",
            OrderStatus.WaitingForCostApproval => "Weryfikacja kosztów",
            OrderStatus.ApprovedByAdmin => "Zatwierdzone (Faktura)",
            OrderStatus.Cancelled => "Anulowane",
            _ => status.ToString()
        };

        public static MudBlazor.Color ToColor(this OrderStatus status) => status switch
        {
            OrderStatus.Created => MudBlazor.Color.Info,
            OrderStatus.PendingApproval => MudBlazor.Color.Warning,
            OrderStatus.Assigned => MudBlazor.Color.Primary,
            OrderStatus.OrderStarted => MudBlazor.Color.Secondary,
            OrderStatus.ApprovedByAdmin => MudBlazor.Color.Success,
            OrderStatus.Cancelled => MudBlazor.Color.Error,
            _ => MudBlazor.Color.Default
        };
    }
}