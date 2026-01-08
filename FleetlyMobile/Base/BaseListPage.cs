namespace FleetlyMobile.Base
{
    public abstract class BaseListPage<TItem> : BasePage
    {
        protected List<TItem> Items { get; set; } = [];
        protected int TotalItems { get; set; } = 0;

        protected int CurrentPage { get; set; } = 1;
        protected int PageSize { get; set; } = 10;

        protected string SearchString { get; set; } = string.Empty;

        protected string SortBy { get; set; } = "date";
        protected bool SortDescending { get; set; } = true;

        protected abstract Task LoadDataAsync();

        protected override async Task OnInitializedAsync()
        {
            await ExecuteSafeAsync(LoadDataAsync);
        }

        protected async Task OnPageChanged(int page)
        {
            CurrentPage = page;
            await ExecuteSafeAsync(LoadDataAsync);
        }

        protected async Task OnSearch(string text)
        {
            SearchString = text;
            CurrentPage = 1;
            await ExecuteSafeAsync(LoadDataAsync);
        }

        protected async Task OnSortChanged(string sortBy, bool descending)
        {
            SortBy = sortBy;
            SortDescending = descending;
            CurrentPage = 1;
            await ExecuteSafeAsync(LoadDataAsync);
        }
    }
}