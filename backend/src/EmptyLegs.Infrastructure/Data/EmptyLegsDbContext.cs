using EmptyLegs.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EmptyLegs.Infrastructure.Data;

public class EmptyLegsDbContext : DbContext
{
    public EmptyLegsDbContext(DbContextOptions<EmptyLegsDbContext> options) : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<User> Users { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Aircraft> Aircraft { get; set; }
    public DbSet<Airport> Airports { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Passenger> Passengers { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<UserAlert> UserAlerts { get; set; }
    public DbSet<BookingService> BookingServices { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global query filter for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Company>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Aircraft>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Airport>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Flight>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Booking>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Passenger>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Payment>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Document>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Review>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<UserAlert>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<BookingService>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(e => !e.IsDeleted);

        // Configure relationships and constraints
        ConfigureUserRelationships(modelBuilder);
        ConfigureCompanyRelationships(modelBuilder);
        ConfigureFlightRelationships(modelBuilder);
        ConfigureBookingRelationships(modelBuilder);
        ConfigureIndexes(modelBuilder);
    }

    private static void ConfigureUserRelationships(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            // Relationship with Company
            entity.HasOne(e => e.Company)
                  .WithMany(e => e.Users)
                  .HasForeignKey(e => e.CompanyId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureCompanyRelationships(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.License).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ContactEmail).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ContactPhone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Website).HasMaxLength(255);

            entity.HasIndex(e => e.License).IsUnique();
            entity.HasIndex(e => e.ContactEmail);
        });
    }

    private static void ConfigureFlightRelationships(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FlightNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.BasePrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CurrentPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.MinimumPrice).HasColumnType("decimal(18,2)");

            // Relationships
            entity.HasOne(e => e.DepartureAirport)
                  .WithMany(e => e.DepartureFlights)
                  .HasForeignKey(e => e.DepartureAirportId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ArrivalAirport)
                  .WithMany(e => e.ArrivalFlights)
                  .HasForeignKey(e => e.ArrivalAirportId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Aircraft)
                  .WithMany(e => e.Flights)
                  .HasForeignKey(e => e.AircraftId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Company)
                  .WithMany(e => e.Flights)
                  .HasForeignKey(e => e.CompanyId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.DepartureTime);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.DepartureAirportId, e.ArrivalAirportId });
        });

        modelBuilder.Entity<Aircraft>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Manufacturer).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Registration).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CruiseSpeed).HasColumnType("decimal(8,2)");
            entity.Property(e => e.Range).HasColumnType("decimal(10,2)");

            entity.HasIndex(e => e.Registration).IsUnique();

            entity.HasOne(e => e.Company)
                  .WithMany(e => e.Aircraft)
                  .HasForeignKey(e => e.CompanyId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IataCode).IsRequired().HasMaxLength(3);
            entity.Property(e => e.IcaoCode).IsRequired().HasMaxLength(4);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TimeZone).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Latitude).HasColumnType("decimal(10,8)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(11,8)");

            entity.HasIndex(e => e.IataCode).IsUnique();
            entity.HasIndex(e => e.IcaoCode).IsUnique();
        });
    }

    private static void ConfigureBookingRelationships(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BookingReference).IsRequired().HasMaxLength(20);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ServiceFees).HasColumnType("decimal(18,2)");

            entity.HasIndex(e => e.BookingReference).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.BookingDate);

            entity.HasOne(e => e.Flight)
                  .WithMany(e => e.Bookings)
                  .HasForeignKey(e => e.FlightId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User)
                  .WithMany(e => e.Bookings)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Passenger>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PassportNumber).HasMaxLength(20);
            entity.Property(e => e.Nationality).HasMaxLength(50);

            entity.HasOne(e => e.Booking)
                  .WithMany(e => e.Passengers)
                  .HasForeignKey(e => e.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StripePaymentIntentId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.RefundAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);

            entity.HasIndex(e => e.StripePaymentIntentId).IsUnique();

            entity.HasOne(e => e.Booking)
                  .WithMany(e => e.Payments)
                  .HasForeignKey(e => e.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Additional performance indexes
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Comment).HasMaxLength(2000);
            entity.Property(e => e.Rating).IsRequired();

            entity.HasIndex(e => e.Rating);
            entity.HasIndex(e => e.IsVisible);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Flight)
                  .WithMany(e => e.Reviews)
                  .HasForeignKey(e => e.FlightId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Company)
                  .WithMany(e => e.Reviews)
                  .HasForeignKey(e => e.CompanyId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<UserAlert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DepartureAirportCode).HasMaxLength(3);
            entity.Property(e => e.ArrivalAirportCode).HasMaxLength(3);
            entity.Property(e => e.MaxPrice).HasColumnType("decimal(18,2)");

            entity.HasIndex(e => e.IsActive);

            entity.HasOne(e => e.User)
                  .WithMany(e => e.Alerts)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FileUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Passenger)
                  .WithMany(e => e.Documents)
                  .HasForeignKey(e => e.PassengerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BookingService>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ServiceType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Booking)
                  .WithMany(e => e.AdditionalServices)
                  .HasForeignKey(e => e.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps before saving
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Deleted:
                    // Implement soft delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}