using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Enums;
using FluentAssertions;
using Xunit;

namespace EmptyLegs.Tests.Unit.Domain;

public class FlightTests
{
    [Fact]
    public void Flight_WhenCreated_ShouldHaveCorrectDefaultValues()
    {
        // Arrange & Act
        var flight = new Flight();

        // Assert
        flight.Id.Should().NotBeEmpty();
        flight.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        flight.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        flight.IsDeleted.Should().BeFalse();
        flight.Status.Should().Be(FlightStatus.Available);
        flight.AllowsAutomaticPricing.Should().BeTrue();
        flight.FlightNumber.Should().BeEmpty();
        flight.Bookings.Should().NotBeNull().And.BeEmpty();
        flight.Reviews.Should().NotBeNull().And.BeEmpty();
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(8, 0, 0)]
    [InlineData(8, 4, 0.5)]
    [InlineData(8, 8, 1.0)]
    public void OccupancyRate_ShouldCalculateCorrectly(int totalSeats, int bookedSeats, decimal expectedRate)
    {
        // Arrange
        var flight = new Flight
        {
            TotalSeats = totalSeats
        };

        // Simulate booked seats with confirmed bookings
        for (int i = 0; i < bookedSeats; i++)
        {
            flight.Bookings.Add(new Booking
            {
                Status = BookingStatus.Confirmed,
                PassengerCount = 1
            });
        }

        // Act
        var occupancyRate = flight.OccupancyRate;

        // Assert
        occupancyRate.Should().Be(expectedRate);
    }

    [Fact]
    public void BookedSeats_ShouldOnlyCountConfirmedAndPaymentConfirmedBookings()
    {
        // Arrange
        var flight = new Flight();
        flight.Bookings.Add(new Booking { Status = BookingStatus.Confirmed, PassengerCount = 2 });
        flight.Bookings.Add(new Booking { Status = BookingStatus.PaymentConfirmed, PassengerCount = 3 });
        flight.Bookings.Add(new Booking { Status = BookingStatus.Pending, PassengerCount = 1 }); // Should not count
        flight.Bookings.Add(new Booking { Status = BookingStatus.Cancelled, PassengerCount = 1 }); // Should not count

        // Act
        var bookedSeats = flight.BookedSeats;

        // Assert
        bookedSeats.Should().Be(5); // 2 + 3 = 5
    }

    [Fact]
    public void Duration_ShouldCalculateCorrectly()
    {
        // Arrange
        var departureTime = new DateTime(2024, 3, 15, 14, 30, 0);
        var arrivalTime = new DateTime(2024, 3, 15, 16, 15, 0);
        
        var flight = new Flight
        {
            DepartureTime = departureTime,
            ArrivalTime = arrivalTime
        };

        // Act
        var duration = flight.Duration;

        // Assert
        duration.Should().Be(TimeSpan.FromMinutes(105)); // 1h45
    }

    [Theory]
    [InlineData(8, 8, true)]
    [InlineData(8, 7, false)]
    [InlineData(0, 0, false)] // Edge case: no seats
    public void IsFullyBooked_ShouldReturnCorrectValue(int availableSeats, int bookedSeats, bool expectedResult)
    {
        // Arrange
        var flight = new Flight
        {
            AvailableSeats = availableSeats
        };

        // Simulate booked seats
        for (int i = 0; i < bookedSeats; i++)
        {
            flight.Bookings.Add(new Booking
            {
                Status = BookingStatus.Confirmed,
                PassengerCount = 1
            });
        }

        // Act
        var isFullyBooked = flight.IsFullyBooked;

        // Assert
        isFullyBooked.Should().Be(expectedResult);
    }

    [Fact]
    public void Flight_WithValidData_ShouldBeCreatedSuccessfully()
    {
        // Arrange
        var flightNumber = "EL001";
        var departureTime = DateTime.UtcNow.AddDays(1);
        var arrivalTime = departureTime.AddHours(2);
        var basePrice = 2500m;
        var currentPrice = 2000m;
        var availableSeats = 6;
        var companyId = Guid.NewGuid();
        var aircraftId = Guid.NewGuid();
        var departureAirportId = Guid.NewGuid();
        var arrivalAirportId = Guid.NewGuid();

        // Act
        var flight = new Flight
        {
            FlightNumber = flightNumber,
            DepartureTime = departureTime,
            ArrivalTime = arrivalTime,
            BasePrice = basePrice,
            CurrentPrice = currentPrice,
            AvailableSeats = availableSeats,
            TotalSeats = availableSeats,
            CompanyId = companyId,
            AircraftId = aircraftId,
            DepartureAirportId = departureAirportId,
            ArrivalAirportId = arrivalAirportId,
            Status = FlightStatus.Available
        };

        // Assert
        flight.FlightNumber.Should().Be(flightNumber);
        flight.DepartureTime.Should().Be(departureTime);
        flight.ArrivalTime.Should().Be(arrivalTime);
        flight.BasePrice.Should().Be(basePrice);
        flight.CurrentPrice.Should().Be(currentPrice);
        flight.AvailableSeats.Should().Be(availableSeats);
        flight.TotalSeats.Should().Be(availableSeats);
        flight.CompanyId.Should().Be(companyId);
        flight.AircraftId.Should().Be(aircraftId);
        flight.DepartureAirportId.Should().Be(departureAirportId);
        flight.ArrivalAirportId.Should().Be(arrivalAirportId);
        flight.Status.Should().Be(FlightStatus.Available);
    }

    [Fact]
    public void Flight_ShouldSupportPricingUpdates()
    {
        // Arrange
        var flight = new Flight
        {
            BasePrice = 3000m,
            CurrentPrice = 3000m,
            AllowsAutomaticPricing = true,
            MinimumPrice = 2000m
        };

        // Act
        flight.CurrentPrice = 2500m; // Dynamic pricing update

        // Assert
        flight.BasePrice.Should().Be(3000m);
        flight.CurrentPrice.Should().Be(2500m);
        flight.MinimumPrice.Should().Be(2000m);
        flight.AllowsAutomaticPricing.Should().BeTrue();
    }
}