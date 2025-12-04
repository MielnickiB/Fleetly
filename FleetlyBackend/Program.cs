using FleetlyBackend.Data;
using FleetlyBackend.Helpers;
using FleetlyBackend.Models;
using FleetlyBackend.Services.AuthService;
using FleetlyBackend.Services.AvailabilityService;
using FleetlyBackend.Services.BrandModelService;
using FleetlyBackend.Services.CarBrandService;
using FleetlyBackend.Services.CostLimitService;
using FleetlyBackend.Services.DamageService;
using FleetlyBackend.Services.ExpenseService;
using FleetlyBackend.Services.FileService;
using FleetlyBackend.Services.InvoiceService;
using FleetlyBackend.Services.LocationService;
using FleetlyBackend.Services.UserDetailsService;
using FleetlyBackend.Services.UserRoleService;
using FleetlyBackend.Services.UserSerivce;
using FleetlyBackend.Services.VehicleService;
using FleetlyBackend.Services.NotificationService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json.Serialization;
using FleetlyBackend.Services.OrderService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Fleetly API",
        Version = "v1",
        Description = "Dokumentacja API"
    });

    // Definicja JWT (Bearer)
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Przykladowo: \"Bearer {token}\"",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });

    // Automatyczne wymaganie autoryzacji dla endpointow z [Authorize]
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddDbContext<FleetlyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging());

builder.Services.Configure<FileUploadOptions>(builder.Configuration.GetSection("FileUpload"));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserDetailsService, UserDetailsService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ICarBrandService, CarBrandService>();
builder.Services.AddScoped<IBrandModelService, BrandModelService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<ICostLimitService, CostLimitService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IDamageService, DamageService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            IssuerSigningKey = SecretKeyHelper.GetSymmetricSecurityKey(builder.Configuration),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5251",
                "https://localhost:5251",
                "http://localhost:5113",
                "https://localhost:5114")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FleetlyContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher<User>>();

        await context.Database.MigrateAsync();
        await DbSeeder.SeedAsync(context, passwordHasher);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Wystapil problem z seedowaniem bazy danych: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
