using AutoMapper;
using EmptyLegs.Application.DTOs;
using EmptyLegs.Application.Mappings;
using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Enums;
using FluentAssertions;
using Xunit;

namespace EmptyLegs.Tests.Unit.Mappings;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void AutoMapper_Configuration_Should_Be_Valid()
    {
        // Act & Assert
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void Should_Map_User_To_UserDto()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+33123456789",
            DateOfBirth = new DateTime(1990, 5, 15),
            Role = UserRole.Customer,
            IsActive = true,
            IsEmailVerified = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var userDto = _mapper.Map<UserDto>(user);

        // Assert
        userDto.Should().NotBeNull();
        userDto.Id.Should().Be(user.Id);
        userDto.Email.Should().Be(user.Email);
        userDto.FirstName.Should().Be(user.FirstName);
        userDto.LastName.Should().Be(user.LastName);
        userDto.PhoneNumber.Should().Be(user.PhoneNumber);
        userDto.DateOfBirth.Should().Be(user.DateOfBirth);
        userDto.Role.Should().Be(user.Role);
        userDto.IsActive.Should().Be(user.IsActive);
        userDto.IsEmailVerified.Should().Be(user.IsEmailVerified);
        userDto.CreatedAt.Should().Be(user.CreatedAt);
        userDto.FullName.Should().Be($"{user.FirstName} {user.LastName}");
    }

    [Fact]
    public void Should_Map_CreateUserDto_To_User()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+33123456789",
            DateOfBirth = new DateTime(1990, 5, 15),
            Role = UserRole.Customer
        };

        // Act
        var user = _mapper.Map<User>(createUserDto);

        // Assert
        user.Should().NotBeNull();
        user.Email.Should().Be(createUserDto.Email);
        user.FirstName.Should().Be(createUserDto.FirstName);
        user.LastName.Should().Be(createUserDto.LastName);
        user.PhoneNumber.Should().Be(createUserDto.PhoneNumber);
        user.DateOfBirth.Should().Be(createUserDto.DateOfBirth);
        user.Role.Should().Be(createUserDto.Role);
    }

    [Fact]
    public void Should_Map_Flight_To_FlightDto()
    {
        // Arrange
        var departureAirport = new Airport
        {
            Id = Guid.NewGuid(),
            IataCode = "CDG",
            Name = "Charles de Gaulle Airport",
            City = "Paris",
            Country = "France"
        };

        var arrivalAirport = new Airport
        {
            Id = Guid.NewGuid(),
            IataCode = "NCE",
            Name = "Nice Côte d'Azur Airport",
            City = "Nice",
            Country = "France"
        };

        var aircraft = new Aircraft
        {
            Id = Guid.NewGuid(),
            Registration = "F-TEST1",
            Type = AircraftType.LightJet,
            Model = "Citation CJ2",
            Manufacturer = "Cessna"
        };

        var flight = new Flight
        {
            Id = Guid.NewGuid(),
            FlightNumber = "TEST001",
            DepartureTime = DateTime.UtcNow.AddDays(7),
            ArrivalTime = DateTime.UtcNow.AddDays(7).AddHours(2),
            BasePrice = 1500.00m,
            CurrentPrice = 1500.00m,
            TotalSeats = 6,
            Status = FlightStatus.Scheduled,
            DepartureAirport = departureAirport,
            ArrivalAirport = arrivalAirport,
            Aircraft = aircraft,
            Bookings = new List<Booking>()
        };

        // Act
        var flightDto = _mapper.Map<FlightDto>(flight);

        // Assert
        flightDto.Should().NotBeNull();
        flightDto.Id.Should().Be(flight.Id);
        flightDto.FlightNumber.Should().Be(flight.FlightNumber);
        flightDto.DepartureTime.Should().Be(flight.DepartureTime);
        flightDto.ArrivalTime.Should().Be(flight.ArrivalTime);
        flightDto.BasePrice.Should().Be(flight.BasePrice);
        flightDto.CurrentPrice.Should().Be(flight.CurrentPrice);
        flightDto.TotalSeats.Should().Be(flight.TotalSeats);
        flightDto.Status.Should().Be(flight.Status);
        flightDto.DepartureAirport.Should().NotBeNull();
        flightDto.ArrivalAirport.Should().NotBeNull();
        flightDto.Aircraft.Should().NotBeNull();
    }

    [Fact]
    public void Should_Map_CreateFlightDto_To_Flight()
    {
        // Arrange
        var createFlightDto = new CreateFlightDto
        {
            FlightNumber = "TEST001",
            DepartureAirportId = Guid.NewGuid(),
            ArrivalAirportId = Guid.NewGuid(),
            DepartureTime = DateTime.UtcNow.AddDays(7),
            ArrivalTime = DateTime.UtcNow.AddDays(7).AddHours(2),
            AircraftId = Guid.NewGuid(),
            BasePrice = 1500.00m,
            AvailableSeats = 6
        };

        // Act
        var flight = _mapper.Map<Flight>(createFlightDto);

        // Assert
        flight.Should().NotBeNull();
        flight.FlightNumber.Should().Be(createFlightDto.FlightNumber);
        flight.DepartureAirportId.Should().Be(createFlightDto.DepartureAirportId);
        flight.ArrivalAirportId.Should().Be(createFlightDto.ArrivalAirportId);
        flight.DepartureTime.Should().Be(createFlightDto.DepartureTime);
        flight.ArrivalTime.Should().Be(createFlightDto.ArrivalTime);
        flight.AircraftId.Should().Be(createFlightDto.AircraftId);
        flight.BasePrice.Should().Be(createFlightDto.BasePrice);
        flight.TotalSeats.Should().Be(createFlightDto.AvailableSeats);
    }

    [Fact]
    public void Should_Map_Company_To_CompanyDto()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Aviation Company",
            Description = "A test aviation company",
            License = "TEST-LICENSE-001",
            ContactEmail = "contact@testaviation.com",
            ContactPhone = "+33123456789",
            Address = "123 Test Street, Paris, France",
            IsActive = true,
            IsVerified = true,
            AverageRating = 4.5,
            TotalReviews = 150,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var companyDto = _mapper.Map<CompanyDto>(company);

        // Assert
        companyDto.Should().NotBeNull();
        companyDto.Id.Should().Be(company.Id);
        companyDto.Name.Should().Be(company.Name);
        companyDto.Description.Should().Be(company.Description);
        companyDto.License.Should().Be(company.License);
        companyDto.ContactEmail.Should().Be(company.ContactEmail);
        companyDto.ContactPhone.Should().Be(company.ContactPhone);
        companyDto.Address.Should().Be(company.Address);
        companyDto.IsActive.Should().Be(company.IsActive);
        companyDto.IsVerified.Should().Be(company.IsVerified);
        companyDto.AverageRating.Should().Be(company.AverageRating);
        companyDto.TotalReviews.Should().Be(company.TotalReviews);
    }

    [Fact]
    public void Should_Map_Booking_To_BookingDto()
    {
        // Arrange
        var flight = new Flight
        {
            Id = Guid.NewGuid(),
            FlightNumber = "TEST001"
        };

        var passengers = new List<Passenger>
        {
            new() { FirstName = "John", LastName = "Doe" },
            new() { FirstName = "Jane", LastName = "Smith" }
        };

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingReference = "EL12345678",
            Status = BookingStatus.Confirmed,
            BookingDate = DateTime.UtcNow,
            PassengerCount = 2,
            BaseAmount = 1500.00m,
            ServiceFees = 75.00m,
            TaxAmount = 120.00m,
            Flight = flight,
            Passengers = passengers
        };

        // Act
        var bookingDto = _mapper.Map<BookingDto>(booking);

        // Assert
        bookingDto.Should().NotBeNull();
        bookingDto.Id.Should().Be(booking.Id);
        bookingDto.BookingReference.Should().Be(booking.BookingReference);
        bookingDto.Status.Should().Be(booking.Status);
        bookingDto.BookingDate.Should().Be(booking.BookingDate);
        bookingDto.PassengerCount.Should().Be(booking.PassengerCount);
        bookingDto.BaseAmount.Should().Be(booking.BaseAmount);
        bookingDto.ServiceFees.Should().Be(booking.ServiceFees);
        bookingDto.TaxAmount.Should().Be(booking.TaxAmount);
        bookingDto.Flight.Should().NotBeNull();
        bookingDto.Passengers.Should().HaveCount(2);
    }

    [Fact]
    public void Should_Map_Review_To_ReviewDto_With_UserName()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe"
        };

        var review = new Review
        {
            Id = Guid.NewGuid(),
            Rating = 5,
            Comment = "Excellent service!",
            IsVerified = true,
            IsVisible = true,
            UserId = user.Id,
            User = user,
            FlightId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var reviewDto = _mapper.Map<ReviewDto>(review);

        // Assert
        reviewDto.Should().NotBeNull();
        reviewDto.Id.Should().Be(review.Id);
        reviewDto.Rating.Should().Be(review.Rating);
        reviewDto.Comment.Should().Be(review.Comment);
        reviewDto.IsVerified.Should().Be(review.IsVerified);
        reviewDto.IsVisible.Should().Be(review.IsVisible);
        reviewDto.UserId.Should().Be(review.UserId);
        reviewDto.UserName.Should().Be("John Doe");
        reviewDto.FlightId.Should().Be(review.FlightId);
        reviewDto.CreatedAt.Should().Be(review.CreatedAt);
    }
}