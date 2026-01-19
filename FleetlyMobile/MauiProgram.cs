using Fleetly.Shared.Client;
using FleetlyMobile.Services;
using FleetlyMobile.Services.Dashboard;
using FleetlyMobile.Services.Auth;
using FleetlyMobile.Constants;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using FleetlyMobile.Services.User;
using FleetlyMobile.Services.Orders;
using FleetlyMobile.Services.Expense;
using FleetlyMobile.Services.Protocol;
using FleetlyMobile.Services.Notifications;
using Microsoft.Maui.Devices.Sensors;
using FleetlyMobile.Services.Availability;

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

            builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);

            builder.Services.AddAuthorizationCore();

            builder.Services.AddSingleton<CustomAuthStateProvider>();
            builder.Services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());

            builder.Services.AddScoped<TokenHandler>();

            builder.Services.AddHttpClient<ApiClient>(client =>
            {
                client.BaseAddress = new Uri(AppConstants.ApiUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<TokenHandler>();

            builder.Services.AddSingleton<LayoutService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IExpenseService, ExpenseService>();
            builder.Services.AddScoped<IProtocolService, ProtocolService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();

            return builder.Build();
        }
    }
}
