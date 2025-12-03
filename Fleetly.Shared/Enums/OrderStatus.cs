namespace Fleetly.Shared.Enums
{
    public enum OrderStatus
    {
        PendingApproval = 1,          // Utworzone przez klienta
        Created = 2,                  // Admin zatwierdził zlecenie (ujawnione dla pracowników)
        Assigned = 3,                 // Pracownik przyjął zlecenie
        OrderStarted = 4,             // Pracownik rozpoczął zlecenie
        ArrivedAtServiceLocation = 5, // Pracownik dotarł do lokalizacji serwisowej (jeśli dotyczy)
        LeftServiceLocation = 6,      // Pracownik opuścił lokalizację serwisową (jeśli dotyczy)
        ArrivedToClient = 7,          // Pracownik dotarł do lokalizacji końcowej
        OrderFinishedByWorker = 8,    // Pracownik zakończył zlecenie
        WaitingForCostApproval = 9,   // Pracownik zgłosił koszty, oczekiwanie na zatwierdzenie przez admina
        ApprovedByAdmin = 10,         // Admin zatwierdził koszty i całkowicie zakończył zlecenie
        Cancelled = 11                // Zlecenie odrzucone przez admina
    }
}
