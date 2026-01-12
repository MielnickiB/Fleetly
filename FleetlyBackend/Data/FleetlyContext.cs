using Microsoft.EntityFrameworkCore;
using FleetlyBackend.Models;

namespace FleetlyBackend.Data
{
    public class FleetlyContext(DbContextOptions<FleetlyContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<CarBrand> CarBrands { get; set; } = null!;
        public DbSet<BrandModel> BrandModels { get; set; } = null!;
        public DbSet<Vehicle> Vehicles { get; set; } = null!;
        public DbSet<Availability> Availabilities { get; set; } = null!;
        public DbSet<CostLimit> CostLimits { get; set; } = null!;
        public DbSet<Damage> Damages { get; set; } = null!;
        public DbSet<Expense> Expenses { get; set; } = null!;
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<Payroll> Payrolls { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Protocol> Protocols { get; set; } = null!;
        public DbSet<ProtocolPhoto> ProtocolPhotos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // UserRole 1:N User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // User 1:1 UserDetails
            modelBuilder.Entity<User>()
                .HasOne(u => u.Details)
                .WithOne(d => d.User)
                .HasForeignKey<UserDetails>(d => d.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // UserDetails UserId unikalny
            modelBuilder.Entity<UserDetails>()
                .HasIndex(d => d.UserId)
                .IsUnique();

            // Email unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // User 1:N Availability
            modelBuilder.Entity<Availability>()
                .HasOne(a => a.Worker)
                .WithMany(w => w.WorkerAvailabilities)
                .HasForeignKey(a => a.WorkerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // User 1:N Vehicle
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.User)
                .WithMany(u => u.Vehicles)
                .HasForeignKey(v => v.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // User 1:N Location
            modelBuilder.Entity<Location>()
                .HasOne(l => l.User)
                .WithMany(u => u.Locations)
                .HasForeignKey(l => l.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // CarBrand 1:N BrandModel
            modelBuilder.Entity<CarBrand>()
                .HasMany(cb => cb.Models)
                .WithOne(m => m.CarBrand)
                .HasForeignKey(m => m.CarBrandId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // BrandModel 1:N Vehicle
            modelBuilder.Entity<BrandModel>()
                .HasMany(bm => bm.Vehicles)
                .WithOne(v => v.BrandModel)
                .HasForeignKey(v => v.BrandModelId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicle RegistrationNumber unikalny
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.RegistrationNumber)
                .IsUnique();

            // Client 1:N Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Client)
                .WithMany(u => u.ClientOrders)
                .HasForeignKey(o => o.ClientId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Worker 1:N Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Worker)
                .WithMany(u => u.WorkerOrders)
                .HasForeignKey(o => o.WorkerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Vehicle 1:N Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Vehicle)
                .WithMany(v => v.Orders)
                .HasForeignKey(o => o.VehicleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Order 1:N Protocol
            modelBuilder.Entity<Protocol>()
                .HasOne(p => p.Order)
                .WithMany()
                .HasForeignKey(p => p.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Worker 1:N Protocol
            modelBuilder.Entity<Protocol>()
                .HasOne(p => p.Worker)
                .WithMany()
                .HasForeignKey(p => p.WorkerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Client 1:N Protocol
            modelBuilder.Entity<Protocol>()
                .HasOne(p => p.Client)
                .WithMany()
                .HasForeignKey(p => p.ClientId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // StartLocation 1:N Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.StartLocation)
                .WithMany()
                .HasForeignKey(o => o.StartLocationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // EndLocation 1:N Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.EndLocation)
                .WithMany()
                .HasForeignKey(o => o.EndLocationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // CostLimit 1:N Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.CostLimit)
                .WithMany()
                .HasForeignKey(o => o.CostLimitId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .Navigation(u => u.Role)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Navigation(u => u.Details)
                .IsRequired();

            modelBuilder.Entity<Expense>()
                .Property(e => e.Cost)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.Salary)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.AdditionalCosts)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.FuelCosts)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Location>(entity =>
            {
                entity.Property(e => e.IsPublic).HasDefaultValue(false);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
