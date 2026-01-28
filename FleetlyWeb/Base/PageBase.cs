using Microsoft.AspNetCore.Components;

namespace FleetlyWeb.Base
{
    public abstract class PageBase<TItem> : ComponentBase
    {
        protected IEnumerable<TItem> Items = [];
        protected int TotalItems = 0;
        protected bool IsLoading = false;
        protected string ErrorMessage = string.Empty;
        protected string SearchString = string.Empty;
        protected bool ShowInactive = false;
        protected abstract Task LoadDataAsync();

        protected async Task OnShowInactiveChanged(bool value)
        {
            ShowInactive = value;
            await LoadDataAsync();
        }
    }
}
