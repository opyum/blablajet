using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Enums;
using FluentAssertions;
using Xunit;

namespace EmptyLegs.Tests.Unit.Entities;

public class FlightTests
{
    [Fact]
    public void Flight_BookedSeats_Should_Calculate_From_Bookings()
    {
        // Arrange
        var flight = new Flight
        {
            TotalSeats = 10,
            Bookings = new List<Booking>
            {
                new() { PassengerCount = 2, Status = BookingStatus.Confirmed },
                new() { PassengerCount = 3, Status = BookingStatus.Confirmed },
                new() { PassengerCount = 1, Status = BookingStatus.Pending },
                new() { PassengerCount = 2, Status = BookingStatus.Cancelled } // Should not count
            }
        };

        // Act
        var bookedSeats = flight.BookedSeats;

        // Assert
        bookedSeats.Should().Be(6); // 2 + 3 + 1 = 6 (cancelled booking not counted)
    }

    [Fact]
    public void Flight_AvailableSeats_Should_Calculate_Remaining_Seats()
    {
        // Arrange
        var flight = new Flight
        {
            TotalSeats = 10,
            Bookings = new List<Booking>
            {
                new() { PassengerCount = 3, Status = BookingStatus.Confirmed },
                new() { PassengerCount = 2, Status = BookingStatus.Pending }
            }
        };

        // Act
        var availableSeats = flight.AvailableSeats;

        // Assert
        availableSeats.Should().Be(5); // 10 - (3 + 2) = 5
    }

    [Fact]
    public void Flight_IsFullyBooked_Should_Return_True_When_No_Available_Seats()
    {
        // Arrange
        var flight = new Flight
        {
            TotalSeats = 5,
            Bookings = new List<Booking>
            {
                new() { PassengerCount = 5, Status = BookingStatus.Confirmed }
            }
        };

        // Act
        var isFullyBooked = flight.IsFullyBooked;

        // Assert
        isFullyBooked.Should().BeTrue();
    }

    [Fact]
    public void Flight_IsFullyBooked_Should_Return_False_When_Seats_Available()
    {
        // Arrange
        var flight = new Flight
        {
            TotalSeats = 10,
            Bookings = new List<Booking>
            {
                new() { PassengerCount = 3, Status = BookingStatus.Confirmed }
            }
        };

        // Act
        var isFullyBooked = flight.IsFullyBooked;

        // Assert
        isFullyBooked.Should().BeFalse();
    }

    [Fact]
    public void Flight_OccupancyRate_Should_Calculate_Percentage()
    {
        // Arrange
        var flight = new Flight
        {
            TotalSeats = 10,
            Bookings = new List<Booking>
            {
                new() { PassengerCount = 3, Status = BookingStatus.Confirmed },
                new() { PassengerCount = 2, Status = BookingStatus.Pending }
            }
        };

        // Act
        var occupancyRate = flight.OccupancyRate;

        // Assert
        occupancyRate.Should().Be(0.5m); // 5/10 = 0.5 (50%)
    }

    [Fact]
    public void Flight_OccupancyRate_Should_Return_Zero_When_No_Bookings()
    {
        // Arrange
        var flight = new Flight
        {
            TotalSeats = 10,
            Bookings = new List<Booking>()
        };

        // Act
        var occupancyRate = flight.OccupancyRate;

        // Assert
        occupancyRate.Should().Be(0m);
    }

    [Fact]
    public void Flight_Duration_Should_Calculate_Flight_Time()
    {
        // Arrange
        var departureTime = new DateTime(2024, 3, 15, 10, 0, 0);
        var arrivalTime = new DateTime(2024, 3, 15, 12, 30, 0);
        
        var flight = new Flight
        {
            DepartureTime = departureTime,
            ArrivalTime = arrivalTime
        };

        // Act
        var duration = flight.Duration;

        // Assert
        duration.Should().Be(TimeSpan.FromHours(2.5)); // 2 hours 30 minutes
    }

    [Fact]
    public void Flight_Duration_Should_Handle_Same_Day_Flight()
    {
        // Arrange
        var departureTime = new DateTime(2024, 3, 15, 23, 0, 0);
        var arrivalTime = new DateTime(2024, 3, 16, 1, 0, 0); // Next day
        
        var flight = new Flight
        {
            DepartureTime = departureTime,
            ArrivalTime = arrivalTime
        };

        // Act
        var duration = flight.Duration;

        // Assert
        duration.Should().Be(TimeSpan.FromHours(2)); // 2 hours across midnight
    }

    [Fact]
    public void Flight_Should_Have_Default_Values()
    {
        // Act
        var flight = new Flight();

        // Assert
        flight.Id.Should().NotBe(Guid.Empty);
        flight.FlightNumber.Should().Be(string.Empty);
        flight.BasePrice.Should().Be(0);
        flight.CurrentPrice.Should().Be(0);
        flight.TotalSeats.Should().Be(0);
        flight.Status.Should().Be(FlightStatus.Available);
        flight.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        flight.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        flight.IsDeleted.Should().BeFalse();
        flight.DeletedAt.Should().BeNull();
        flight.Bookings.Should().NotBeNull().And.BeEmpty();
        flight.Reviews.Should().NotBeNull().And.BeEmpty();
    }

    [Theory]
    [InlineData(FlightStatus.Available)]
    [InlineData(FlightStatus.Reserved)]
    [InlineData(FlightStatus.Confirmed)]
    [InlineData(FlightStatus.InProgress)]
    [InlineData(FlightStatus.Completed)]
    [InlineData(FlightStatus.Cancelled)]
    public void Flight_Should_Accept_Valid_Status(FlightStatus status)
    {
        // Arrange & Act
        var flight = new Flight { Status = status };

        // Assert
        flight.Status.Should().Be(status);
    }

    [Fact]
    public void Flight_Should_Allow_Setting_Valid_Price()
    {
        // Arrange
        var basePrice = 1500.50m;
        var currentPrice = 1750.00m;

        // Act
        var flight = new Flight
        {
            BasePrice = basePrice,
            CurrentPrice = currentPrice
        };

        // Assert
        flight.BasePrice.Should().Be(basePrice);
        flight.CurrentPrice.Should().Be(currentPrice);
    }
}