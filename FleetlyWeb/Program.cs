using FleetlyWeb;
using FleetlyWeb.Services;
using FleetlyWeb.Services.BrandModel;
using FleetlyWeb.Services.CarBrand;
using FleetlyWeb.Services.Users;
using FleetlyWeb.Services.UserRoles;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Fleetly.Shared.Client;
using FleetlyWeb.Services.Locations;
using FleetlyWeb.Services.Orders;
using FleetlyWeb.Services.Notifications;
using FleetlyWeb.Services.CostLimits;
using FleetlyWeb.Services.Dashboard;
using FleetlyWeb.Services.Vehicles;
using FleetlyWeb.Services.Authorization;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiUrl = builder.Configuration.GetValue<string>("ApiUrl") ?? "http://192.168.0.104:5225/";

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<TokenHandler>();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(apiUrl);
})
.AddHttpMessageHandler<TokenHandler>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IBrandModelService, BrandModelService>();
builder.Services.AddScoped<ICarBrandService, CarBrandService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICostLimitService, CostLimitService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

await builder.Build().RunAsync();
