using MudBlazor;

namespace FleetlyWeb.Theme;

public static class FleetlyTheme
{
    public static readonly MudTheme Default = new() 
    { 
        PaletteLight = new PaletteLight()
        {
            Primary = "#1E88E5",
            Secondary = "#1565C0",
            Background = "#F5F7FA",
            Surface = "#FFFFFF",
            AppbarBackground = "#1565C0",
            TextPrimary = "#1F2937",
            DrawerBackground = "#1F2A37",
            DrawerText = "#E5E7EB",
            ActionDefault = "#1E88E5"
        },

        PaletteDark = new PaletteDark()
        {
            Primary = "#64B5F6",
            Secondary = "#1E88E5",
            Background = "#111827",
            Surface = "#1F2937",
            DrawerBackground = "#0F172A",
            DrawerText = "#E2E8F0"
        },
        Typography = new Typography()
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Inter", "sans-serif"]
            }
        }

    };
}
