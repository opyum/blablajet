using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Enums;
using FluentAssertions;
using Xunit;

namespace EmptyLegs.Tests.Unit.Domain;

public class BookingTests
{
    [Fact]
    public void Booking_WhenCreated_ShouldHaveCorrectDefaultValues()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        booking.Id.Should().NotBeEmpty();
        booking.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        booking.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        booking.IsDeleted.Should().BeFalse();
        booking.Status.Should().Be(BookingStatus.Pending);
        booking.BookingDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        booking.BookingReference.Should().BeEmpty();
        booking.Passengers.Should().NotBeNull().And.BeEmpty();
        booking.Payments.Should().NotBeNull().And.BeEmpty();
        booking.AdditionalServices.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void TotalAmount_ShouldCalculateCorrectly()
    {
        // Arrange
        var booking = new Booking
        {
            TotalPrice = 2500m,
            ServiceFees = 150m
        };

        booking.AdditionalServices.Add(new BookingService
        {
            Price = 100m,
            Quantity = 2 // Total: 200
        });

        booking.AdditionalServices.Add(new BookingService
        {
            Price = 50m,
            Quantity = 1 // Total: 50
        });

        // Act
        var totalAmount = booking.TotalAmount;

        // Assert
        totalAmount.Should().Be(2900m); // 2500 + 150 + 200 + 50 = 2900
    }

    [Theory]
    [InlineData(BookingStatus.Pending, true)]
    [InlineData(BookingStatus.Confirmed, true)]
    [InlineData(BookingStatus.PaymentConfirmed, false)]
    [InlineData(BookingStatus.Cancelled, false)]
    [InlineData(BookingStatus.Completed, false)]
    public void CanBeCancelled_ShouldReturnCorrectValue(BookingStatus status, bool expectedResult)
    {
        // Arrange
        var booking = new Booking
        {
            Status = status,
            BookingDate = DateTime.UtcNow.AddHours(-2) // Booked 2 hours ago
        };

        // Act
        var canBeCancelled = booking.CanBeCancelled;

        // Assert
        canBeCancelled.Should().Be(expectedResult);
    }

    [Fact]
    public void CanBeCancelled_WhenBookedMoreThan24HoursAgo_ShouldReturnFalse()
    {
        // Arrange
        var booking = new Booking
        {
            Status = BookingStatus.Confirmed,
            BookingDate = DateTime.UtcNow.AddDays(-2) // Booked 2 days ago
        };

        // Act
        var canBeCancelled = booking.CanBeCancelled;

        // Assert
        canBeCancelled.Should().BeFalse();
    }

    [Fact]
    public void GenerateBookingReference_ShouldCreateValidReference()
    {
        // Arrange
        var booking = new Booking();

        // Act
        var reference = booking.GenerateBookingReference();

        // Assert
        reference.Should().NotBeNullOrEmpty();
        reference.Should().StartWith("EL");
        reference.Length.Should().Be(10); // EL + 8 characters
        booking.BookingReference.Should().Be(reference);
    }

    [Fact]
    public void GenerateBookingReference_ShouldCreateUniqueReferences()
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
    public void Booking_WithPassengers_ShouldMaintainCorrectRelationships()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            PassengerCount = 2
        };

        var passenger1 = new Passenger
        {
            FirstName = "John",
            LastName = "Doe",
            BookingId = bookingId
        };

        var passenger2 = new Passenger
        {
            FirstName = "Jane",
            LastName = "Smith",
            BookingId = bookingId
        };

        // Act
        booking.Passengers.Add(passenger1);
        booking.Passengers.Add(passenger2);

        // Assert
        booking.Passengers.Should().HaveCount(2);
        booking.PassengerCount.Should().Be(2);
        booking.Passengers.Should().Contain(p => p.FirstName == "John" && p.LastName == "Doe");
        booking.Passengers.Should().Contain(p => p.FirstName == "Jane" && p.LastName == "Smith");
    }

    [Fact]
    public void Booking_WithPayments_ShouldTrackPaymentHistory()
    {
        // Arrange
        var booking = new Booking
        {
            TotalPrice = 2500m
        };

        var payment1 = new Payment
        {
            Amount = 1000m,
            Status = "succeeded",
            PaymentMethod = "card"
        };

        var payment2 = new Payment
        {
            Amount = 1500m,
            Status = "succeeded",
            PaymentMethod = "card"
        };

        // Act
        booking.Payments.Add(payment1);
        booking.Payments.Add(payment2);

        // Assert
        booking.Payments.Should().HaveCount(2);
        booking.Payments.Sum(p => p.Amount).Should().Be(2500m);
        booking.Payments.Should().AllSatisfy(p => p.Status.Should().Be("succeeded"));
    }

    [Fact]
    public void Booking_CancellationWorkflow_ShouldUpdateStatusAndTimestamp()
    {
        // Arrange
        var booking = new Booking
        {
            Status = BookingStatus.Confirmed
        };

        var cancellationReason = "Client request";

        // Act
        booking.Status = BookingStatus.Cancelled;
        booking.CancellationReason = cancellationReason;
        booking.CancelledAt = DateTime.UtcNow;

        // Assert
        booking.Status.Should().Be(BookingStatus.Cancelled);
        booking.CancellationReason.Should().Be(cancellationReason);
        booking.CancelledAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Booking_WithSpecialRequests_ShouldStoreCorrectly()
    {
        // Arrange
        var specialRequests = "Vegetarian meal, wheelchair assistance";
        
        var booking = new Booking
        {
            SpecialRequests = specialRequests
        };

        // Act & Assert
        booking.SpecialRequests.Should().Be(specialRequests);
    }
}