using EmptyLegs.Application.DTOs;
using EmptyLegs.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmptyLegs.Tests.Integration.ApiTests;

public class FlightsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public FlightsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task InitializeAsync()
    {
        await _factory.SeedDataAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetFlights_Should_Return_Flights_List()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/flights");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var flights = JsonSerializer.Deserialize<List<FlightDto>>(content, _jsonOptions);
        
        flights.Should().NotBeNull();
        flights.Should().HaveCountGreaterThan(0);
        flights!.First().FlightNumber.Should().Be("TEST001");
    }

    [Fact]
    public async Task GetFlight_Should_Return_Flight_When_Exists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmptyLegsDbContext>();
        var existingFlight = await context.Flights.FirstAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/flights/{existingFlight.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var flight = JsonSerializer.Deserialize<FlightDto>(content, _jsonOptions);
        
        flight.Should().NotBeNull();
        flight!.Id.Should().Be(existingFlight.Id);
        flight.FlightNumber.Should().Be("TEST001");
    }

    [Fact]
    public async Task GetFlight_Should_Return_NotFound_When_Not_Exists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/flights/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchFlights_Should_Return_Filtered_Results()
    {
        // Arrange
        var searchCriteria = new FlightSearchDto
        {
            DepartureAirportCode = "CDG",
            ArrivalAirportCode = "NCE",
            DepartureDate = DateTime.UtcNow.AddDays(7),
            PassengerCount = 2
        };

        var queryString = $"?departureAirportCode={searchCriteria.DepartureAirportCode}" +
                         $"&arrivalAirportCode={searchCriteria.ArrivalAirportCode}" +
                         $"&departureDate={searchCriteria.DepartureDate?.ToString("yyyy-MM-dd")}" +
                         $"&passengerCount={searchCriteria.PassengerCount}";

        // Act
        var response = await _client.GetAsync($"/api/v1/flights/search{queryString}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var searchResult = JsonSerializer.Deserialize<FlightSearchResultDto>(content, _jsonOptions);
        
        searchResult.Should().NotBeNull();
        searchResult!.Flights.Should().HaveCountGreaterThan(0);
        searchResult.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateFlight_Should_Create_Flight_Successfully()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmptyLegsDbContext>();
        var company = await context.Companies.FirstAsync();
        var aircraft = await context.Aircraft.FirstAsync();
        var departureAirport = await context.Airports.FirstAsync(a => a.IataCode == "CDG");
        var arrivalAirport = await context.Airports.FirstAsync(a => a.IataCode == "NCE");

        var createFlightDto = new CreateFlightDto
        {
            FlightNumber = "TEST002",
            DepartureAirportId = departureAirport.Id,
            ArrivalAirportId = arrivalAirport.Id,
            DepartureTime = DateTime.UtcNow.AddDays(14),
            ArrivalTime = DateTime.UtcNow.AddDays(14).AddHours(2),
            AircraftId = aircraft.Id,
            BasePrice = 2000.00m,
            AvailableSeats = 6
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/flights", createFlightDto, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        var createdFlight = JsonSerializer.Deserialize<FlightDto>(content, _jsonOptions);
        
        createdFlight.Should().NotBeNull();
        createdFlight!.FlightNumber.Should().Be("TEST002");
        createdFlight.BasePrice.Should().Be(2000.00m);
        
        // Verify the flight was actually saved to the database
        var savedFlight = await context.Flights.FindAsync(createdFlight.Id);
        savedFlight.Should().NotBeNull();
        savedFlight!.FlightNumber.Should().Be("TEST002");
    }

    [Fact]
    public async Task CreateFlight_Should_Return_BadRequest_For_Invalid_Data()
    {
        // Arrange
        var invalidFlightDto = new CreateFlightDto
        {
            FlightNumber = "", // Invalid empty flight number
            DepartureTime = DateTime.UtcNow.AddDays(-1), // Past date
            BasePrice = -100 // Negative price
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/flights", invalidFlightDto, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateFlight_Should_Update_Flight_Successfully()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmptyLegsDbContext>();
        var existingFlight = await context.Flights.FirstAsync();

        var updateFlightDto = new UpdateFlightDto
        {
            BasePrice = 1800.00m,
            Status = EmptyLegs.Core.Enums.FlightStatus.Confirmed
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/flights/{existingFlight.Id}", updateFlightDto, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var updatedFlight = JsonSerializer.Deserialize<FlightDto>(content, _jsonOptions);
        
        updatedFlight.Should().NotBeNull();
        updatedFlight!.BasePrice.Should().Be(1800.00m);
        updatedFlight.Status.Should().Be(EmptyLegs.Core.Enums.FlightStatus.Confirmed);
    }

    [Fact]
    public async Task UpdateFlight_Should_Return_NotFound_When_Flight_Not_Exists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateFlightDto = new UpdateFlightDto
        {
            BasePrice = 1800.00m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/flights/{nonExistentId}", updateFlightDto, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteFlight_Should_Soft_Delete_Flight_Successfully()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmptyLegsDbContext>();
        var existingFlight = await context.Flights.FirstAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/flights/{existingFlight.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify the flight was soft deleted
        var deletedFlight = await context.Flights.IgnoreQueryFilters()
            .FirstOrDefaultAsync(f => f.Id == existingFlight.Id);
        deletedFlight.Should().NotBeNull();
        deletedFlight!.IsDeleted.Should().BeTrue();
        deletedFlight.DeletedAt.Should().NotBeNull();
        
        // Verify the flight is not returned by normal queries
        var normalQuery = await context.Flights.FirstOrDefaultAsync(f => f.Id == existingFlight.Id);
        normalQuery.Should().BeNull();
    }

    [Fact]
    public async Task GetFlightAvailability_Should_Return_Availability_Info()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmptyLegsDbContext>();
        var existingFlight = await context.Flights.FirstAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/flights/{existingFlight.Id}/availability");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var availability = JsonSerializer.Deserialize<FlightAvailabilityDto>(content, _jsonOptions);
        
        availability.Should().NotBeNull();
        availability!.FlightId.Should().Be(existingFlight.Id);
        availability.AvailableSeats.Should().BeGreaterThan(0);
        availability.TotalSeats.Should().BeGreaterThan(0);
    }
}