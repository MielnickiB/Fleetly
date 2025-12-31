namespace Fleetly.Shared.Enums
{
    public enum OrderStatus
    {
        PendingApproval = 1,          // Utworzone przez klienta
        Created = 2,                  // Admin zatwierdził zlecenie (ujawnione dla pracowników)
        Assigned = 3,                 // Pracownik przyjął zlecenie
        OrderStarted = 4,             // Pracownik rozpoczął zlecenie
        ArrivedToClient = 5,          // Pracownik dotarł do lokalizacji końcowej
        OrderFinishedByWorker = 6,    // Pracownik zakończył zlecenie
        WaitingForCostApproval = 7,   // Pracownik zgłosił koszty, oczekiwanie na zatwierdzenie przez admina
        ApprovedByAdmin = 8,          // Admin zatwierdził koszty i całkowicie zakończył zlecenie
        Cancelled = 9                 // Zlecenie odrzucone przez admina
    }
}
