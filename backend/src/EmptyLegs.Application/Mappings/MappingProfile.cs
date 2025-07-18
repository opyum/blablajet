using AutoMapper;
using EmptyLegs.Application.DTOs;
using EmptyLegs.Core.Entities;

namespace EmptyLegs.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();

        // Company mappings
        CreateMap<Company, CompanyDto>();
        CreateMap<CreateCompanyDto, Company>();
        CreateMap<UpdateCompanyDto, Company>();

        // Aircraft mappings
        CreateMap<Aircraft, AircraftDto>();
        CreateMap<CreateAircraftDto, Aircraft>();
        CreateMap<UpdateAircraftDto, Aircraft>();

        // Airport mappings
        CreateMap<Airport, AirportDto>();
        CreateMap<CreateAirportDto, Airport>();

        // Flight mappings
        CreateMap<Flight, FlightDto>()
            .ForMember(dest => dest.DepartureAirport, opt => opt.MapFrom(src => src.DepartureAirport))
            .ForMember(dest => dest.ArrivalAirport, opt => opt.MapFrom(src => src.ArrivalAirport))
            .ForMember(dest => dest.Aircraft, opt => opt.MapFrom(src => src.Aircraft));
        CreateMap<CreateFlightDto, Flight>();
        CreateMap<UpdateFlightDto, Flight>();

        // Booking mappings
        CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.Flight, opt => opt.MapFrom(src => src.Flight))
            .ForMember(dest => dest.Passengers, opt => opt.MapFrom(src => src.Passengers));
        CreateMap<CreateBookingDto, Booking>();

        // Passenger mappings
        CreateMap<Passenger, PassengerDto>();
        CreateMap<CreatePassengerDto, Passenger>();

        // Payment mappings
        CreateMap<Payment, PaymentDto>();
        CreateMap<CreatePaymentDto, Payment>();

        // Review mappings
        CreateMap<Review, ReviewDto>();
        CreateMap<CreateReviewDto, Review>();
    }
}