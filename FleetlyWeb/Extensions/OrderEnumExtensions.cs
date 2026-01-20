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
            OrderStatus.ArrivedToClient => "U klienta",
            OrderStatus.OrderFinishedByWorker => "Zakończone (Czeka na koszty)",
            OrderStatus.WaitingForCostApproval => "Weryfikacja kosztów",
            OrderStatus.ApprovedByAdmin => "Zatwierdzone (Faktura)",
            OrderStatus.Cancelled => "Anulowane",
            _ => status.ToString()
        };

        public static string ToHexColor(this OrderStatus status) => status switch
        {
            OrderStatus.Created => "#64DD17",

            OrderStatus.PendingApproval => "#FFA726",

            OrderStatus.Assigned => "#64B5F6",

            OrderStatus.OrderStarted => "#1E88E5",

            OrderStatus.ArrivedToClient => "#1565C0",

            OrderStatus.OrderFinishedByWorker => "#42A5F5",

            OrderStatus.WaitingForCostApproval => "#00E676",

            OrderStatus.ApprovedByAdmin => "#00C853",

            OrderStatus.Cancelled => "#F44336",

            _ => "#9E9E9E"
        };
    }
}