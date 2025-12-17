using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Validations
{
    /// <summary>
    /// Tworzy walidator roku produkcji pojazdu.
    /// </summary>
    /// <param name="minYearsBack">Ile lat wstecz od dzisiaj uznajemy za ważny rok (domyślnie 30)</param>
    [AttributeUsage(AttributeTargets.Property)]
    public class VehicleYearValidation(int minYearsBack = 30) : ValidationAttribute
    {
        private readonly int _minYearsBack = minYearsBack;

        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            if (!int.TryParse(value.ToString(), out int year))
                return false;

            var currentYear = DateTime.UtcNow.Year;
            var minYear = currentYear - _minYearsBack;

            return year >= minYear && year <= currentYear;
        }

        public override string FormatErrorMessage(string name)
        {
            var currentYear = DateTime.UtcNow.Year;
            var minYear = currentYear - _minYearsBack;

            return $"Rok produkcji musi być pomiędzy {minYear} a {currentYear}.";
        }
    }
}