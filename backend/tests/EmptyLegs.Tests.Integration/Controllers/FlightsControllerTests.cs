using EmptyLegs.Application.DTOs;
using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Enums;
using EmptyLegs.Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace EmptyLegs.Tests.Integration.Controllers;

public class FlightsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly EmptyLegsDbContext _context;

    public FlightsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<EmptyLegsDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing
                services.AddDbContext<EmptyLegsDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
                });
            });
        });

        _client = _factory.CreateClient();
        
        // Get the DbContext from the test server
        var scope = _factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<EmptyLegsDbContext>();
        
        // Seed test data
        SeedTestData().Wait();
    }

    private async Task SeedTestData()
    {
        // Create test airports
        var cdgAirport = new Airport
        {
            Id = Guid.NewGuid(),
            IataCode = "CDG",
            IcaoCode = "LFPG",
            Name = "Charles de Gaulle Airport",
            City = "Paris",
            Country = "France",
            Latitude = 49.0097m,
            Longitude = 2.5479m,
            TimeZone = "Europe/Paris"
        };

        var nceAirport = new Airport
        {
            Id = Guid.NewGuid(),
            IataCode = "NCE",
            IcaoCode = "LFMN",
            Name = "Nice Côte d'Azur Airport",
            City = "Nice",
            Country = "France",
            Latitude = 43.6584m,
            Longitude = 7.2159m,
            TimeZone = "Europe/Paris"
        };

        // Create test company
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Aviation",
            Description = "Test company for integration tests",
            License = "TEST123",
            ContactEmail = "test@testaviation.com",
            ContactPhone = "+33123456789",
            Address = "123 Test Street",
            City = "Paris",
            Country = "France",
            IsVerified = true,
            IsActive = true
        };

        // Create test aircraft
        var aircraft = new Aircraft
        {
            Id = Guid.NewGuid(),
            Model = "Citation CJ3+",
            Manufacturer = "Cessna",
            Registration = "F-TEST",
            Capacity = 8,
            Type = AircraftType.LightJet,
            YearManufactured = 2020,
            Description = "Test aircraft",
            IsActive = true,
            CompanyId = company.Id
        };

        // Create test flights
        var flight1 = new Flight
        {
            Id = Guid.NewGuid(),
            FlightNumber = "TEST001",
            DepartureTime = DateTime.UtcNow.AddDays(1),
            ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(2),
            BasePrice = 2500m,
            CurrentPrice = 2000m,
            AvailableSeats = 6,
            TotalSeats = 8,
            Status = FlightStatus.Available,
            CompanyId = company.Id,
            AircraftId = aircraft.Id,
            DepartureAirportId = cdgAirport.Id,
            ArrivalAirportId = nceAirport.Id
        };

        var flight2 = new Flight
        {
            Id = Guid.NewGuid(),
            FlightNumber = "TEST002",
            DepartureTime = DateTime.UtcNow.AddDays(2),
            ArrivalTime = DateTime.UtcNow.AddDays(2).AddHours(2),
            BasePrice = 3000m,
            CurrentPrice = 2500m,
            AvailableSeats = 4,
            TotalSeats = 8,
            Status = FlightStatus.Available,
            CompanyId = company.Id,
            AircraftId = aircraft.Id,
            DepartureAirportId = nceAirport.Id,
            ArrivalAirportId = cdgAirport.Id
        };

        _context.Airports.AddRange(cdgAirport, nceAirport);
        _context.Companies.Add(company);
        _context.Aircraft.Add(aircraft);
        _context.Flights.AddRange(flight1, flight2);
        
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetFlights_ShouldReturnFlights()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/flights/search");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<FlightSearchResultDto>(content, options);
        
        result.Should().NotBeNull();
        result!.Flights.Should().HaveCountGreaterThan(0);
        result.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SearchFlights_WithDepartureAirport_ShouldReturnFilteredResults()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/flights/search?departureAirportCode=CDG");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<FlightSearchResultDto>(content, options);
        
        result.Should().NotBeNull();
        result!.Flights.Should().HaveCount(1);
        result.Flights.First().DepartureAirport.IataCode.Should().Be("CDG");
    }

    [Fact]
    public async Task SearchFlights_WithArrivalAirport_ShouldReturnFilteredResults()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/flights/search?arrivalAirportCode=NCE");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<FlightSearchResultDto>(content, options);
        
        result.Should().NotBeNull();
        result!.Flights.Should().HaveCount(1);
        result.Flights.First().ArrivalAirport.IataCode.Should().Be("NCE");
    }

    [Fact]
    public async Task SearchFlights_WithPriceFilter_ShouldReturnFilteredResults()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/flights/search?maxPrice=2200");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<FlightSearchResultDto>(content, options);
        
        result.Should().NotBeNull();
        result!.Flights.Should().HaveCount(1);
        result.Flights.First().CurrentPrice.Should().BeLessOrEqualTo(2200m);
    }

    [Fact]
    public async Task SearchFlights_WithPassengerCount_ShouldReturnFilteredResults()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/flights/search?passengerCount=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<FlightSearchResultDto>(content, options);
        
        result.Should().NotBeNull();
        result!.Flights.Should().HaveCount(1);
        result.Flights.First().AvailableSeats.Should().BeGreaterOrEqualTo(5);
    }

    [Fact]
    public async Task SearchFlights_WithPagination_ShouldReturnCorrectPage()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/flights/search?page=1&pageSize=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<FlightSearchResultDto>(content, options);
        
        result.Should().NotBeNull();
        result!.Flights.Should().HaveCount(1);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(1);
        result.TotalCount.Should().BeGreaterOrEqualTo(1);
        result.HasNextPage.Should().Be(result.TotalCount > 1);
    }

    [Fact]
    public async Task SearchFlights_WithSorting_ShouldReturnSortedResults()
    {
        // Act - Sort by price ascending
        var response = await _client.GetAsync("/api/v1/flights/search?sortBy=price&sortDescending=false");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<FlightSearchResultDto>(content, options);
        
        result.Should().NotBeNull();
        result!.Flights.Should().HaveCountGreaterThan(1);
        
        // Verify sorting
        var prices = result.Flights.Select(f => f.CurrentPrice).ToList();
        prices.Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetFlight_WithValidId_ShouldReturnFlight()
    {
        // Arrange
        var flight = _context.Flights.First();

        // Act
        var response = await _client.GetAsync($"/api/v1/flights/{flight.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<FlightDto>(content, options);
        
        result.Should().NotBeNull();
        result!.Id.Should().Be(flight.Id);
        result.FlightNumber.Should().Be(flight.FlightNumber);
    }

    [Fact]
    public async Task GetFlight_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/flights/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchFlights_WithMultipleFilters_ShouldReturnCorrectResults()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/flights/search?departureAirportCode=CDG&arrivalAirportCode=NCE&maxPrice=2500&passengerCount=4");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<FlightSearchResultDto>(content, options);
        
        result.Should().NotBeNull();
        result!.Flights.Should().HaveCount(1);
        
        var flight = result.Flights.First();
        flight.DepartureAirport.IataCode.Should().Be("CDG");
        flight.ArrivalAirport.IataCode.Should().Be("NCE");
        flight.CurrentPrice.Should().BeLessOrEqualTo(2500m);
        flight.AvailableSeats.Should().BeGreaterOrEqualTo(4);
    }

    public void Dispose()
    {
        _context.Dispose();
        _client.Dispose();
    }
}