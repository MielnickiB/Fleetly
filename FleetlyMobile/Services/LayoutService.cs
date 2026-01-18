namespace FleetlyMobile.Services
{
    public class LayoutService
    {
        public bool IsDarkMode { get; private set; }
        public event Action? OnMajorUpdateOccurred;

        public void SetDarkMode(bool value)
        {
            IsDarkMode = value;
            OnMajorUpdateOccurred?.Invoke();
        }
    }
}
