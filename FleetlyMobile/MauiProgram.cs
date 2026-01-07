using Fleetly.Shared.Client;
using FleetlyMobile.Services;
using FleetlyMobile.Services.Dashboard;
using FleetlyMobile.Services.Auth;
using FleetlyMobile.Constants;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace FleetlyMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            builder.Services.AddMudServices();

            builder.Services.AddAuthorizationCore();

            builder.Services.AddScoped<CustomAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());

            builder.Services.AddTransient<TokenHandler>();

            builder.Services.AddHttpClient<ApiClient>(client =>
            {
                client.BaseAddress = new Uri(AppConstants.ApiUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<TokenHandler>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();

            return builder.Build();
        }
    }
}
