namespace Fleetly.Shared.Enums
{
    public enum ProtocolType
    {
        Pickup = 0, // Wydanie (Odbiór od klienta)
        Return = 1  // Zwrot (Odbiór przez klienta)
    }

    public enum VehicleSide
    {
        Front = 0,
        Back = 1,
        Left = 2,
        Right = 3,
        Interior = 4
    }

    public enum DamageType
    {
        Scratch, // Rysa
        Dent, // Wgniecenie
        Crack, // Pęknięcie
        Abrasion, // Otarcie
        MissingPart, // Brak elementu
        Dirty, // Zabrudzenie
        Other // Inne
    }

    public enum DamagePart
    {
        BumperFront, BumperBack, // Zderzak Przedni, Zderzak Tylny
        LampLeft, LampRight, // Lampa Lewa, Lampa Prawa
        DoorFront, DoorBack, // Drzwi Przednie, Drzwi Tylne
        FenderFront, FenderBack, // Błotnik Przedni, Błotnik Tylny
        Hood, Roof, Trunk, Windshield, // Maska, Dach, Bagażnik, Szyba Przednia
        MirrorLeft, MirrorRight, // Lusterko Lewe, Lusterko Prawe
        WheelFront, WheelBack, // Koło Przednie, Koło Tylne
        TireFront, TireBack, // Opona Przednia, Opona Tylna
        Upholstery, Dashboard, SteeringWheel, // Tapicerka, Deska Rozdzielcza, Kierownica
        Other // Inne
    }
}