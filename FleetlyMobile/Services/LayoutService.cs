namespace FleetlyMobile.Services
{
    public class LayoutService
    {
        public bool IsDarkMode { get; private set; }
        public event Action? OnMajorUpdateOccured;

        public void SetDarkMode(bool value)
        {
            IsDarkMode = value;
            OnMajorUpdateOccured?.Invoke();
        }
    }
}
