namespace FleetlyBackend.Config
{
    public static class PaginationOptions
    {
        public static int DefaultPage { get; } = 1;
        public static int DefaultPageSize { get; } = 1;
        public static int MaxPageSize { get; } = 20;
    }
}
