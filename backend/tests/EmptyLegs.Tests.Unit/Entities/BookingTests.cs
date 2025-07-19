using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Enums;
using FluentAssertions;
using Xunit;

namespace EmptyLegs.Tests.Unit.Entities;

public class BookingTests
{
    [Fact]
    public void Booking_Should_Have_Valid_Default_Values()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        booking.Id.Should().NotBeEmpty();
        booking.BookingReference.Should().NotBeNull();
        booking.Status.Should().Be(BookingStatus.Pending);
        booking.BookingDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        booking.PassengerCount.Should().Be(0);
        booking.TotalPrice.Should().Be(0);
        booking.ServiceFees.Should().Be(0);
        booking.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Booking_TotalAmount_Should_Calculate_From_All_Components()
    {
        // Arrange
        var booking = new Booking
        {
            TotalPrice = 1000.00m,
            ServiceFees = 50.00m
        };

        // Act
        var totalAmount = booking.TotalAmount;

        // Assert
        totalAmount.Should().Be(1050.00m);
    }

    [Fact]
    public void Booking_CanBeCancelled_Should_Return_True_For_Pending_Status()
    {
        // Arrange
        var booking = new Booking
        {
            Status = BookingStatus.Pending
        };

        // Act
        var canBeCancelled = booking.CanBeCancelled;

        // Assert
        canBeCancelled.Should().BeTrue();
    }

    [Fact]
    public void Booking_CanBeCancelled_Should_Return_True_For_Confirmed_Status()
    {
        // Arrange
        var booking = new Booking
        {
            Status = BookingStatus.Confirmed
        };

        // Act
        var canBeCancelled = booking.CanBeCancelled;

        // Assert
        canBeCancelled.Should().BeTrue();
    }

    [Fact]
    public void Booking_CanBeCancelled_Should_Return_False_For_Completed_Status()
    {
        // Arrange
        var booking = new Booking
        {
            Status = BookingStatus.Completed
        };

        // Act
        var canBeCancelled = booking.CanBeCancelled;

        // Assert
        canBeCancelled.Should().BeFalse();
    }

    [Fact]
    public void Booking_GenerateBookingReference_Should_Create_Valid_Reference()
    {
        // Arrange
        var booking = new Booking();

        // Act
        var reference = booking.GenerateBookingReference();

        // Assert
        reference.Should().NotBeNullOrEmpty();
        reference.Should().StartWith("EL");
        reference.Length.Should().Be(10);
        booking.BookingReference.Should().Be(reference);
    }

    [Fact]
    public void Booking_Should_Support_Multiple_Passengers()
    {
        // Arrange
        var booking = new Booking();
        var passenger1 = new Passenger { FirstName = "John", LastName = "Doe" };
        var passenger2 = new Passenger { FirstName = "Jane", LastName = "Smith" };

        // Act
        booking.Passengers.Add(passenger1);
        booking.Passengers.Add(passenger2);

        // Assert
        booking.Passengers.Should().HaveCount(2);
        booking.Passengers.Should().Contain(passenger1);
        booking.Passengers.Should().Contain(passenger2);
    }

    [Fact]
    public void Booking_Should_Support_Multiple_Payments()
    {
        // Arrange
        var booking = new Booking();
        var payment1 = new Payment { Amount = 500m, PaymentMethod = "CreditCard" };
        var payment2 = new Payment { Amount = 300m, PaymentMethod = "BankTransfer" };

        // Act
        booking.Payments.Add(payment1);
        booking.Payments.Add(payment2);

        // Assert
        booking.Payments.Should().HaveCount(2);
        booking.Payments.Should().Contain(payment1);
        booking.Payments.Should().Contain(payment2);
    }

    [Fact]
    public void Booking_Should_Have_Correct_Navigation_Properties()
    {
        // Arrange
        var booking = new Booking();

        // Assert
        booking.Passengers.Should().NotBeNull();
        booking.Payments.Should().NotBeNull();
        booking.AdditionalServices.Should().NotBeNull();
    }
}