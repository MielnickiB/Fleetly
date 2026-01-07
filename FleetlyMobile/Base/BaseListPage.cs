namespace FleetlyMobile.Base
{
    public abstract class FleetlyListPageBase<TItem> : BasePage
    {
        protected List<TItem> Items { get; set; } = [];
        protected int TotalItems { get; set; } = 0;

        protected int CurrentPage { get; set; } = 1;
        protected int PageSize { get; set; } = 10;

        protected string SearchString { get; set; } = string.Empty;

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
    }
}