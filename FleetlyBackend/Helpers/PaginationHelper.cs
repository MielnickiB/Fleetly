using FleetlyBackend.Config;

namespace FleetlyBackend.Helpers
{
    public static class PaginationHelper
    {
        public static (int skip, int take) Calculate(int page, int pageSize)
        {
            if (page < 1)
                page = PaginationOptions.DefaultPage;

            if (pageSize < 1)
                pageSize = PaginationOptions.DefaultPageSize;

            var safe = Math.Min(pageSize, PaginationOptions.MaxPageSize);
            var skip = (page - 1) * safe;

            return (skip, safe);
        }
    }
}
