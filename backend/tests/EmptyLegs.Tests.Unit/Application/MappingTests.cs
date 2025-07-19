using AutoMapper;
using EmptyLegs.Application.DTOs;
using EmptyLegs.Application.Mappings;
using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Enums;
using FluentAssertions;
using Xunit;

namespace EmptyLegs.Tests.Unit.Application;

public class MappingTests
{
    private readonly IMapper _mapper;

    public MappingTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void AutoMapper_Configuration_ShouldBeValid()
    {
        // Act & Assert
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void Flight_ToFlightDto_ShouldMapCorrectly()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Aviation",
            Description = "Test company",
            License = "TEST123"
        };

        var aircraft = new Aircraft
        {
            Id = Guid.NewGuid(),
            Model = "Citation CJ3+",
            Manufacturer = "Cessna",
            Registration = "F-TEST",
            Capacity = 8,
            Type = AircraftType.LightJet,
            Company = company
        };

        var departureAirport = new Airport
        {
            Id = Guid.NewGuid(),
            IataCode = "CDG",
            IcaoCode = "LFPG",
            Name = "Charles de Gaulle Airport",
            City = "Paris",
            Country = "France"
        };

        var arrivalAirport = new Airport
        {
            Id = Guid.NewGuid(),
            IataCode = "NCE",
            IcaoCode = "LFMN",
            Name = "Nice Côte d'Azur Airport",
            City = "Nice",
            Country = "France"
        };

        var flight = new Flight
        {
            Id = Guid.NewGuid(),
            FlightNumber = "TEST001",
            DepartureTime = new DateTime(2024, 3, 15, 14, 30, 0),
            ArrivalTime = new DateTime(2024, 3, 15, 16, 15, 0),
            BasePrice = 2500m,
            CurrentPrice = 2000m,
            AvailableSeats = 6,
            TotalSeats = 8,
            Status = FlightStatus.Available,
            Description = "Test flight",
            Company = company,
            Aircraft = aircraft,
            DepartureAirport = departureAirport,
            ArrivalAirport = arrivalAirport,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Add some bookings to test computed properties
        flight.Bookings.Add(new Booking
        {
            Status = BookingStatus.Confirmed,
            PassengerCount = 2
        });

        // Act
        var dto = _mapper.Map<FlightDto>(flight);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(flight.Id);
        dto.FlightNumber.Should().Be(flight.FlightNumber);
        dto.DepartureTime.Should().Be(flight.DepartureTime);
        dto.ArrivalTime.Should().Be(flight.ArrivalTime);
        dto.BasePrice.Should().Be(flight.BasePrice);
        dto.CurrentPrice.Should().Be(flight.CurrentPrice);
        dto.AvailableSeats.Should().Be(flight.AvailableSeats);
        dto.TotalSeats.Should().Be(flight.TotalSeats);
        dto.Status.Should().Be(flight.Status);
        dto.Description.Should().Be(flight.Description);
        dto.BookedSeats.Should().Be(2);
        dto.OccupancyRate.Should().Be(0.25m); // 2/8
        dto.Duration.Should().Be(TimeSpan.FromMinutes(105)); // 1h45
        dto.IsFullyBooked.Should().BeFalse();
        dto.CreatedAt.Should().Be(flight.CreatedAt);
        dto.UpdatedAt.Should().Be(flight.UpdatedAt);

        // Check nested objects
        dto.Company.Should().NotBeNull();
        dto.Company.Id.Should().Be(company.Id);
        dto.Company.Name.Should().Be(company.Name);

        dto.Aircraft.Should().NotBeNull();
        dto.Aircraft.Id.Should().Be(aircraft.Id);
        dto.Aircraft.Model.Should().Be(aircraft.Model);
        dto.Aircraft.CompanyName.Should().Be(company.Name);

        dto.DepartureAirport.Should().NotBeNull();
        dto.DepartureAirport.IataCode.Should().Be(departureAirport.IataCode);

        dto.ArrivalAirport.Should().NotBeNull();
        dto.ArrivalAirport.IataCode.Should().Be(arrivalAirport.IataCode);
    }

    [Fact]
    public void CreateFlightDto_ToFlight_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new CreateFlightDto
        {
            FlightNumber = "TEST002",
            DepartureAirportId = Guid.NewGuid(),
            ArrivalAirportId = Guid.NewGuid(),
            DepartureTime = DateTime.UtcNow.AddDays(1),
            ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(2),
            AircraftId = Guid.NewGuid(),
            BasePrice = 3000m,
            AvailableSeats = 6,
            Description = "Test flight creation",
            SpecialInstructions = "Test instructions",
            AllowsAutomaticPricing = true,
            MinimumPrice = 2000m
        };

        // Act
        var flight = _mapper.Map<Flight>(createDto);

        // Assert
        flight.Should().NotBeNull();
        flight.FlightNumber.Should().Be(createDto.FlightNumber);
        flight.DepartureAirportId.Should().Be(createDto.DepartureAirportId);
        flight.ArrivalAirportId.Should().Be(createDto.ArrivalAirportId);
        flight.DepartureTime.Should().Be(createDto.DepartureTime);
        flight.ArrivalTime.Should().Be(createDto.ArrivalTime.Value);
        flight.AircraftId.Should().Be(createDto.AircraftId);
        flight.BasePrice.Should().Be(createDto.BasePrice);
        flight.CurrentPrice.Should().Be(createDto.BasePrice); // Should be set to BasePrice initially
        flight.AvailableSeats.Should().Be(createDto.AvailableSeats);
        flight.TotalSeats.Should().Be(createDto.AvailableSeats); // Should be set to AvailableSeats initially
        flight.Description.Should().Be(createDto.Description);
        flight.SpecialInstructions.Should().Be(createDto.SpecialInstructions);
        flight.AllowsAutomaticPricing.Should().Be(createDto.AllowsAutomaticPricing);
        flight.MinimumPrice.Should().Be(createDto.MinimumPrice);
    }

    [Fact]
    public void Company_ToCompanyDto_ShouldMapCorrectly()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Elite Aviation",
            Description = "Premium private jet services",
            License = "ELI123",
            LogoUrl = "https://example.com/logo.png",
            ContactEmail = "contact@elite.com",
            ContactPhone = "+33123456789",
            Address = "123 Elite Street",
            City = "Paris",
            Country = "France",
            Website = "https://elite.com",
            IsVerified = true,
            IsActive = true,
            AverageRating = 4.8m,
            TotalReviews = 156,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.Map<CompanyDto>(company);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(company.Id);
        dto.Name.Should().Be(company.Name);
        dto.Description.Should().Be(company.Description);
        dto.License.Should().Be(company.License);
        dto.LogoUrl.Should().Be(company.LogoUrl);
        dto.ContactEmail.Should().Be(company.ContactEmail);
        dto.ContactPhone.Should().Be(company.ContactPhone);
        dto.Address.Should().Be(company.Address);
        dto.City.Should().Be(company.City);
        dto.Country.Should().Be(company.Country);
        dto.Website.Should().Be(company.Website);
        dto.IsVerified.Should().Be(company.IsVerified);
        dto.IsActive.Should().Be(company.IsActive);
        dto.AverageRating.Should().Be(company.AverageRating);
        dto.TotalReviews.Should().Be(company.TotalReviews);
        dto.CreatedAt.Should().Be(company.CreatedAt);
    }

    [Fact]
    public void User_ToUserDto_ShouldMapCorrectly()
    {
        // Arrange
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company"
        };

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
            CompanyId = company.Id,
            Company = company,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.Map<UserDto>(user);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(user.Id);
        dto.Email.Should().Be(user.Email);
        dto.FirstName.Should().Be(user.FirstName);
        dto.LastName.Should().Be(user.LastName);
        dto.PhoneNumber.Should().Be(user.PhoneNumber);
        dto.DateOfBirth.Should().Be(user.DateOfBirth);
        dto.Role.Should().Be(user.Role);
        dto.IsActive.Should().Be(user.IsActive);
        dto.IsEmailVerified.Should().Be(user.IsEmailVerified);
        dto.CompanyId.Should().Be(user.CompanyId);
        dto.CompanyName.Should().Be(company.Name);
        dto.FullName.Should().Be(user.FullName);
        dto.CreatedAt.Should().Be(user.CreatedAt);
    }

    [Fact]
    public void Booking_ToBookingDto_ShouldMapCorrectly()
    {
        // Arrange
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingReference = "EL001234",
            Status = BookingStatus.Confirmed,
            PassengerCount = 2,
            TotalPrice = 2500m,
            ServiceFees = 150m,
            BookingDate = DateTime.UtcNow.AddDays(-1),
            SpecialRequests = "Vegetarian meal",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };

        // Add some additional services
        booking.AdditionalServices.Add(new BookingService
        {
            Price = 100m,
            Quantity = 2
        });

        // Act
        var dto = _mapper.Map<BookingDto>(booking);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(booking.Id);
        dto.BookingReference.Should().Be(booking.BookingReference);
        dto.Status.Should().Be(booking.Status);
        dto.PassengerCount.Should().Be(booking.PassengerCount);
        dto.TotalPrice.Should().Be(booking.TotalPrice);
        dto.ServiceFees.Should().Be(booking.ServiceFees);
        dto.BookingDate.Should().Be(booking.BookingDate);
        dto.SpecialRequests.Should().Be(booking.SpecialRequests);
        dto.TotalAmount.Should().Be(2850m); // 2500 + 150 + (100*2) = 2850
        dto.CanBeCancelled.Should().Be(booking.CanBeCancelled);
        dto.CreatedAt.Should().Be(booking.CreatedAt);
        dto.UpdatedAt.Should().Be(booking.UpdatedAt);
    }

    [Fact]
    public void Airport_ToAirportDto_ShouldMapCorrectly()
    {
        // Arrange
        var airport = new Airport
        {
            Id = Guid.NewGuid(),
            IataCode = "CDG",
            IcaoCode = "LFPG",
            Name = "Charles de Gaulle Airport",
            City = "Paris",
            Country = "France",
            Latitude = 49.0097m,
            Longitude = 2.5479m,
            TimeZone = "Europe/Paris",
            IsActive = true
        };

        // Act
        var dto = _mapper.Map<AirportDto>(airport);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(airport.Id);
        dto.IataCode.Should().Be(airport.IataCode);
        dto.IcaoCode.Should().Be(airport.IcaoCode);
        dto.Name.Should().Be(airport.Name);
        dto.City.Should().Be(airport.City);
        dto.Country.Should().Be(airport.Country);
        dto.Latitude.Should().Be(airport.Latitude);
        dto.Longitude.Should().Be(airport.Longitude);
        dto.TimeZone.Should().Be(airport.TimeZone);
        dto.IsActive.Should().Be(airport.IsActive);
    }
}