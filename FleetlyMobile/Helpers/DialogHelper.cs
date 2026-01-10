using MudBlazor;

namespace FleetlyMobile.Helpers
{
    public static class DialogHelper
    {
        public static DialogOptions GetStandardFormOptions() => new()
        {
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            CloseOnEscapeKey = true,
            CloseButton = true,
            BackgroundClass = "blurry-bg"
        };

        public static DialogOptions GetConfirmationOptions() => new()
        {
            MaxWidth = MaxWidth.ExtraSmall,
            CloseButton = true,
            BackdropClick = true,
            CloseOnEscapeKey = true
        };

        public static DialogOptions GetImagePreviewOptions() => new()
        {
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            CloseButton = true,
            BackgroundClass = "blurry-bg"
        };
    }
}