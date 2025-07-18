using AutoMapper;
using EmptyLegs.Application.DTOs;
using EmptyLegs.Core.Entities;

namespace EmptyLegs.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Flight mappings
        CreateMap<Flight, FlightDto>()
            .ForMember(dest => dest.BookedSeats, opt => opt.MapFrom(src => src.BookedSeats))
            .ForMember(dest => dest.OccupancyRate, opt => opt.MapFrom(src => src.OccupancyRate))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
            .ForMember(dest => dest.IsFullyBooked, opt => opt.MapFrom(src => src.IsFullyBooked));

        CreateMap<CreateFlightDto, Flight>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DepartureAirport, opt => opt.Ignore())
            .ForMember(dest => dest.ArrivalAirport, opt => opt.Ignore())
            .ForMember(dest => dest.Aircraft, opt => opt.Ignore())
            .ForMember(dest => dest.Company, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.TotalSeats, opt => opt.MapFrom(src => src.AvailableSeats))
            .ForMember(dest => dest.CurrentPrice, opt => opt.MapFrom(src => src.BasePrice));

        // Company mappings
        CreateMap<Company, CompanyDto>();
        CreateMap<CreateCompanyDto, Company>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsVerified, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.LogoUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Users, opt => opt.Ignore())
            .ForMember(dest => dest.Aircraft, opt => opt.Ignore())
            .ForMember(dest => dest.Flights, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore());

        // Aircraft mappings
        CreateMap<Aircraft, AircraftDto>()
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name));
        
        CreateMap<CreateAircraftDto, Aircraft>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.AmenitiesJson, opt => opt.Ignore())
            .ForMember(dest => dest.PhotoUrlsJson, opt => opt.Ignore())
            .ForMember(dest => dest.CompanyId, opt => opt.Ignore())
            .ForMember(dest => dest.Company, opt => opt.Ignore())
            .ForMember(dest => dest.Flights, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.Amenities = src.Amenities;
                dest.PhotoUrls = src.PhotoUrls;
            });

        // Airport mappings
        CreateMap<Airport, AirportDto>();
        CreateMap<CreateAirportDto, Airport>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.DepartureFlights, opt => opt.Ignore())
            .ForMember(dest => dest.ArrivalFlights, opt => opt.Ignore());

        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company != null ? src.Company.Name : null));
        
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Company, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore())
            .ForMember(dest => dest.Alerts, opt => opt.Ignore());

        // Booking mappings
        CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.CanBeCancelled, opt => opt.MapFrom(src => src.CanBeCancelled));

        CreateMap<CreateBookingDto, Booking>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.BookingReference, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => Core.Enums.BookingStatus.Pending))
            .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.ServiceFees, opt => opt.Ignore())
            .ForMember(dest => dest.Flight, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Passengers, opt => opt.Ignore())
            .ForMember(dest => dest.Payments, opt => opt.Ignore())
            .ForMember(dest => dest.AdditionalServices, opt => opt.Ignore());

        // Passenger mappings
        CreateMap<Passenger, PassengerDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age));

        CreateMap<CreatePassengerDto, Passenger>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.BookingId, opt => opt.Ignore())
            .ForMember(dest => dest.Booking, opt => opt.Ignore())
            .ForMember(dest => dest.Documents, opt => opt.Ignore());

        // Payment mappings
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.IsSuccessful, opt => opt.MapFrom(src => src.IsSuccessful))
            .ForMember(dest => dest.IsRefunded, opt => opt.MapFrom(src => src.IsRefunded));

        // Review mappings
        CreateMap<Review, ReviewDto>();
        CreateMap<CreateReviewDto, Review>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.IsVisible, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Flight, opt => opt.Ignore())
            .ForMember(dest => dest.Company, opt => opt.Ignore());

        // UserAlert mappings
        CreateMap<UserAlert, UserAlertDto>();
        CreateMap<CreateUserAlertDto, UserAlert>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.EmailNotifications, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.PushNotifications, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.SmsNotifications, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        // Document mappings
        CreateMap<Document, DocumentDto>();

        // BookingService mappings
        CreateMap<BookingService, BookingServiceDto>()
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));
    }
}