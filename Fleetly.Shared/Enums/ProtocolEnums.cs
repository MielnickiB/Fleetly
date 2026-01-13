using System.ComponentModel;

namespace Fleetly.Shared.Enums
{
    public enum ProtocolType
    {
        [Description("Odbiór")]
        Pickup = 0,
        [Description("Przyjazd")]
        Delivery = 1
    }

    public enum VehicleSide
    {
        [Description("Przód")]
        Front = 0,
        [Description("Tył")]
        Back = 1,
        [Description("Lewa strona")]
        Left = 2,
        [Description("Prawa strona")]
        Right = 3,
        [Description("Wnętrze")]
        Interior = 4
    }

    public enum DamageType
    {
        [Description("Rysa")]
        Scratch,
        [Description("Wgniecenie")]
        Dent,
        [Description("Pęknięcie")]
        Crack,
        [Description("Przetarcie")]
        Abrasion,
        [Description("Brak elementu")]
        MissingPart,
        [Description("Zabrudzenie")]
        Dirty,
        [Description("Inne")]
        Other
    }

    public enum DamagePart
    {
        [Description("Zderzak Przedni")]
        BumperFront,
        [Description("Zderzak Tylny")]
        BumperBack,
        [Description("Lampa Lewa")]
        LampLeft,
        [Description("Lampa Prawa")]
        LampRight,
        [Description("Drzwi Przednie")]
        DoorFront,
        [Description("Drzwi Tylne")]
        DoorBack,
        [Description("Błotnik Przedni")]
        FenderFront,
        [Description("Błotnik Tylny")]
        FenderBack,
        [Description("Maska")]
        Hood,
        [Description("Dach")]
        Roof,
        [Description("Klapa Bagażnik")]
        Trunk,
        [Description("Szyba Przednia")]
        Windshield,
        [Description("Lusterko Lewe")]
        MirrorLeft,
        [Description("Lusterko Prawe")]
        MirrorRight,
        [Description("Koło Przednie")]
        WheelFront, 
        [Description("Koło Tylne")]
        WheelBack,
        [Description("Opona Przednia")]
        TireFront,
        [Description("Opona Tylna")]
        TireBack,
        [Description("Tapicerka")]
        Upholstery,
        [Description("Deska Rozdzielcza")]
        Dashboard,
        [Description("Kierownica")]
        SteeringWheel,
        [Description("Inne")]
        Other
    }
}