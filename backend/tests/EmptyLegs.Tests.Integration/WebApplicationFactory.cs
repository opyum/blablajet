using EmptyLegs.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmptyLegs.Tests.Integration;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    public string DatabaseName { get; } = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the app's DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<EmptyLegsDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add a database context using an in-memory database for testing
            services.AddDbContext<EmptyLegsDbContext>(options =>
            {
                options.UseInMemoryDatabase(DatabaseName);
                options.EnableSensitiveDataLogging();
            });

            // Remove Redis cache registration for testing
            var redisDescriptor = services.SingleOrDefault(
                d => d.ServiceType.Name.Contains("Redis") || 
                     d.ServiceType.Name.Contains("Cache"));
            
            if (redisDescriptor != null)
            {
                services.Remove(redisDescriptor);
            }

            // Add in-memory cache instead of Redis for testing
            services.AddMemoryCache();

            // Override logging for testing
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsole();
                loggingBuilder.SetMinimumLevel(LogLevel.Warning);
            });
        });

        builder.UseEnvironment("Testing");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Create the host for testing
        return base.CreateHost(builder);
    }

    public async Task<EmptyLegsDbContext> GetDbContextAsync()
    {
        var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmptyLegsDbContext>();
        await context.Database.EnsureCreatedAsync();
        return context;
    }

    public async Task SeedDataAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmptyLegsDbContext>();
        
        await context.Database.EnsureCreatedAsync();
        await SeedTestDataAsync(context);
    }

    private static async Task SeedTestDataAsync(EmptyLegsDbContext context)
    {
        // Clear existing data
        context.Users.RemoveRange(context.Users);
        context.Companies.RemoveRange(context.Companies);
        context.Aircraft.RemoveRange(context.Aircraft);
        context.Airports.RemoveRange(context.Airports);
        context.Flights.RemoveRange(context.Flights);
        
        await context.SaveChangesAsync();

        // Add test data that multiple tests can use
        var testCompany = new EmptyLegs.Core.Entities.Company
        {
            Name = "Test Aviation Company",
            Description = "A test aviation company",
            License = "TEST-LICENSE-001",
            ContactEmail = "contact@testaviation.com",
            ContactPhone = "+33123456789",
            Address = "123 Test Street, Paris, France",
            IsActive = true,
            IsVerified = true
        };

        context.Companies.Add(testCompany);
        await context.SaveChangesAsync();

        var testUser = new EmptyLegs.Core.Entities.User
        {
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Role = EmptyLegs.Core.Enums.UserRole.Customer,
            IsActive = true,
            IsEmailVerified = true
        };

        context.Users.Add(testUser);

        var companyUser = new EmptyLegs.Core.Entities.User
        {
            Email = "company@testaviation.com",
            FirstName = "Company",
            LastName = "Admin",
            Role = EmptyLegs.Core.Enums.UserRole.Company,
            CompanyId = testCompany.Id,
            IsActive = true,
            IsEmailVerified = true
        };

        context.Users.Add(companyUser);

        var airports = new List<EmptyLegs.Core.Entities.Airport>
        {
            new()
            {
                IataCode = "CDG",
                IcaoCode = "LFPG",
                Name = "Charles de Gaulle Airport",
                City = "Paris",
                Country = "France",
                Latitude = 49.0097M,
                Longitude = 2.5479M,
                TimeZone = "Europe/Paris",
                IsActive = true
            },
            new()
            {
                IataCode = "NCE",
                IcaoCode = "LFMN",
                Name = "Nice Côte d'Azur Airport",
                City = "Nice",
                Country = "France",
                Latitude = 43.6584M,
                Longitude = 7.2151M,
                TimeZone = "Europe/Paris",
                IsActive = true
            }
        };

        context.Airports.AddRange(airports);

        var aircraft = new EmptyLegs.Core.Entities.Aircraft
        {
            Registration = "F-TEST1",
            Type = EmptyLegs.Core.Enums.AircraftType.LightJet,
            Model = "Citation CJ2",
            Manufacturer = "Cessna",
            YearManufactured = 2020,
            Capacity = 6,
            CruiseSpeed = 450,
            Range = 2000,
            CompanyId = testCompany.Id,
            IsActive = true
        };

        context.Aircraft.Add(aircraft);
        await context.SaveChangesAsync();

        var flight = new EmptyLegs.Core.Entities.Flight
        {
            FlightNumber = "TEST001",
            DepartureAirportId = airports[0].Id,
            ArrivalAirportId = airports[1].Id,
            DepartureTime = DateTime.UtcNow.AddDays(7),
            ArrivalTime = DateTime.UtcNow.AddDays(7).AddHours(2),
            BasePrice = 1500.00m,
            CurrentPrice = 1500.00m,
            TotalSeats = 6,
            Status = EmptyLegs.Core.Enums.FlightStatus.Available,
            AircraftId = aircraft.Id,
            CompanyId = testCompany.Id
        };

        context.Flights.Add(flight);
        await context.SaveChangesAsync();
    }
}