using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Enums;
using FluentAssertions;
using Xunit;

namespace EmptyLegs.Tests.Unit.Entities;

public class BookingTests
{
    [Fact]
    public void Booking_GenerateBookingReference_Should_Create_Valid_Reference()
    {
        // Arrange
        var booking = new Booking();

        // Act
        var reference = booking.GenerateBookingReference();

        // Assert
        reference.Should().NotBeNullOrEmpty();
        reference.Should().HaveLength(10); // "EL" + 8 characters
        reference.Should().StartWith("EL");
        booking.BookingReference.Should().Be(reference);
    }

    [Fact]
    public void Booking_GenerateBookingReference_Should_Create_Unique_References()
    {
        // Arrange
        var booking1 = new Booking();
        var booking2 = new Booking();

        // Act
        var reference1 = booking1.GenerateBookingReference();
        var reference2 = booking2.GenerateBookingReference();

        // Assert
        reference1.Should().NotBe(reference2);
    }

    [Fact]
    public void Booking_TotalAmount_Should_Calculate_From_Base_Price_And_Fees()
    {
        // Arrange
        var booking = new Booking
        {
            BaseAmount = 1000.00m,
            ServiceFees = 50.00m,
            TaxAmount = 80.00m
        };

        // Act
        var totalAmount = booking.TotalAmount;

        // Assert
        totalAmount.Should().Be(1130.00m); // 1000 + 50 + 80
    }

    [Fact]
    public void Booking_TotalAmount_Should_Handle_Zero_Fees()
    {
        // Arrange
        var booking = new Booking
        {
            BaseAmount = 1000.00m,
            ServiceFees = 0.00m,
            TaxAmount = 0.00m
        };

        // Act
        var totalAmount = booking.TotalAmount;

        // Assert
        totalAmount.Should().Be(1000.00m);
    }

    [Fact]
    public void Booking_CanBeCancelled_Should_Return_True_When_Future_Flight_And_Confirmed()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(7);
        var booking = new Booking
        {
            Status = BookingStatus.Confirmed,
            Flight = new Flight { DepartureTime = futureDate }
        };

        // Act
        var canBeCancelled = booking.CanBeCancelled;

        // Assert
        canBeCancelled.Should().BeTrue();
    }

    [Fact]
    public void Booking_CanBeCancelled_Should_Return_False_When_Flight_Has_Departed()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var booking = new Booking
        {
            Status = BookingStatus.Confirmed,
            Flight = new Flight { DepartureTime = pastDate }
        };

        // Act
        var canBeCancelled = booking.CanBeCancelled;

        // Assert
        canBeCancelled.Should().BeFalse();
    }

    [Fact]
    public void Booking_CanBeCancelled_Should_Return_False_When_Already_Cancelled()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(7);
        var booking = new Booking
        {
            Status = BookingStatus.Cancelled,
            Flight = new Flight { DepartureTime = futureDate }
        };

        // Act
        var canBeCancelled = booking.CanBeCancelled;

        // Assert
        canBeCancelled.Should().BeFalse();
    }

    [Theory]
    [InlineData(BookingStatus.Pending)]
    [InlineData(BookingStatus.Confirmed)]
    public void Booking_CanBeCancelled_Should_Return_True_For_Valid_Status_With_Future_Flight(BookingStatus status)
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(2);
        var booking = new Booking
        {
            Status = status,
            Flight = new Flight { DepartureTime = futureDate }
        };

        // Act
        var canBeCancelled = booking.CanBeCancelled;

        // Assert
        canBeCancelled.Should().BeTrue();
    }

    [Theory]
    [InlineData(BookingStatus.Cancelled)]
    [InlineData(BookingStatus.Completed)]
    public void Booking_CanBeCancelled_Should_Return_False_For_Final_Status(BookingStatus status)
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(7);
        var booking = new Booking
        {
            Status = status,
            Flight = new Flight { DepartureTime = futureDate }
        };

        // Act
        var canBeCancelled = booking.CanBeCancelled;

        // Assert
        canBeCancelled.Should().BeFalse();
    }

    [Fact]
    public void Booking_Should_Have_Default_Values()
    {
        // Act
        var booking = new Booking();

        // Assert
        booking.Id.Should().NotBe(Guid.Empty);
        booking.BookingReference.Should().Be(string.Empty);
        booking.Status.Should().Be(BookingStatus.Pending);
        booking.BookingDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        booking.PassengerCount.Should().Be(0);
        booking.BaseAmount.Should().Be(0);
        booking.ServiceFees.Should().Be(0);
        booking.TaxAmount.Should().Be(0);
        booking.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        booking.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        booking.IsDeleted.Should().BeFalse();
        booking.DeletedAt.Should().BeNull();
        booking.Passengers.Should().NotBeNull().And.BeEmpty();
        booking.Payments.Should().NotBeNull().And.BeEmpty();
        booking.AdditionalServices.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Booking_Should_Set_PassengerCount_Based_On_Passengers_Collection()
    {
        // Arrange
        var booking = new Booking
        {
            Passengers = new List<Passenger>
            {
                new() { FirstName = "John", LastName = "Doe" },
                new() { FirstName = "Jane", LastName = "Smith" },
                new() { FirstName = "Bob", LastName = "Wilson" }
            }
        };

        // Act
        booking.PassengerCount = booking.Passengers.Count;

        // Assert
        booking.PassengerCount.Should().Be(3);
    }

    [Fact]
    public void Booking_Should_Allow_Setting_Valid_Amounts()
    {
        // Arrange
        var baseAmount = 1500.00m;
        var serviceFees = 75.00m;
        var taxAmount = 120.00m;

        // Act
        var booking = new Booking
        {
            BaseAmount = baseAmount,
            ServiceFees = serviceFees,
            TaxAmount = taxAmount
        };

        // Assert
        booking.BaseAmount.Should().Be(baseAmount);
        booking.ServiceFees.Should().Be(serviceFees);
        booking.TaxAmount.Should().Be(taxAmount);
        booking.TotalAmount.Should().Be(1695.00m);
    }
}