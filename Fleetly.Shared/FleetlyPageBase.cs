using Microsoft.AspNetCore.Components;

namespace Fleetly.Shared
{
    public abstract class FleetlyPageBase<TItem> : ComponentBase
    {
        protected IEnumerable<TItem> Items = [];
        protected int TotalItems = 0;
        protected bool IsLoading = false;
        protected string ErrorMessage = string.Empty;
        protected string SearchString = string.Empty;
        protected bool ShowInactive = false;

        protected abstract Task LoadDataAsync();
    }
}
