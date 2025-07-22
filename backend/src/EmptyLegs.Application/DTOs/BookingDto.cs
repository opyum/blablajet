using EmptyLegs.Core.Enums;

namespace EmptyLegs.Application.DTOs;

public class BookingDto
{
    public Guid Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public BookingStatus Status { get; set; }
    public int PassengerCount { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal ServiceFees { get; set; }
    public DateTime BookingDate { get; set; }
    public string? SpecialRequests { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    public Guid FlightId { get; set; }
    public FlightDto Flight { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public UserDto User { get; set; } = null!;
    
    public List<PassengerDto> Passengers { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
    public List<BookingServiceDto> AdditionalServices { get; set; } = new();
    
    public decimal TotalAmount { get; set; }
    public bool CanBeCancelled { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateBookingDto
{
    public Guid FlightId { get; set; }
    public int PassengerCount { get; set; }
    public decimal TotalPrice { get; set; }
    public string? SpecialRequests { get; set; }
    public List<CreatePassengerDto> Passengers { get; set; } = new();
    public List<CreateBookingServiceDto> AdditionalServices { get; set; } = new();
}

public class UpdateBookingStatusDto
{
    public BookingStatus Status { get; set; }
    public string? CancellationReason { get; set; }
    public string? Reason { get; set; }
}

// Yacht Booking DTOs
public class YachtBookingDto
{
    public Guid Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public BookingStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Duration { get; set; }
    public int GuestCount { get; set; }
    public decimal BasePrice { get; set; }
    public decimal ServiceFees { get; set; }
    public decimal AdditionalFees { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime BookingDate { get; set; }
    public string? SpecialRequests { get; set; }
    public string ItineraryNotes { get; set; } = string.Empty;
    public bool IncludesCrew { get; set; }
    public bool IncludesFuel { get; set; }
    public List<string> Extras { get; set; } = new();
    public YachtDto Yacht { get; set; } = null!;
    public UserDto User { get; set; } = null!;
    public bool CanBeCancelled { get; set; }
}

public class CreateYachtBookingDto
{
    public Guid YachtId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int GuestCount { get; set; }
    public string? SpecialRequests { get; set; }
    public string ItineraryNotes { get; set; } = string.Empty;
    public bool IncludesCrew { get; set; } = true;
    public bool IncludesFuel { get; set; } = false;
    public List<string> Extras { get; set; } = new();
}

// Car Booking DTOs
public class CarBookingDto
{
    public Guid Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public BookingStatus Status { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int Hours { get; set; }
    public string PickupLocation { get; set; } = string.Empty;
    public string DropoffLocation { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal ServiceFees { get; set; }
    public decimal AdditionalFees { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime BookingDate { get; set; }
    public string? SpecialRequests { get; set; }
    public bool RequiresDriver { get; set; }
    public string? DriverNotes { get; set; }
    public List<string> Extras { get; set; } = new();
    public LuxuryCarDto Car { get; set; } = null!;
    public UserDto User { get; set; } = null!;
    public bool CanBeCancelled { get; set; }
}

public class CreateCarBookingDto
{
    public Guid CarId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string PickupLocation { get; set; } = string.Empty;
    public string DropoffLocation { get; set; } = string.Empty;
    public string? SpecialRequests { get; set; }
    public bool RequiresDriver { get; set; } = true;
    public string? DriverNotes { get; set; }
    public List<string> Extras { get; set; } = new();
}

// Hotel Booking DTOs
public class HotelBookingDto
{
    public Guid Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public BookingStatus Status { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int Nights { get; set; }
    public int Guests { get; set; }
    public decimal RoomRate { get; set; }
    public decimal ServiceFees { get; set; }
    public decimal AdditionalFees { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime BookingDate { get; set; }
    public string? SpecialRequests { get; set; }
    public bool IncludesBreakfast { get; set; }
    public bool IncludesSpa { get; set; }
    public bool IncludesTransfer { get; set; }
    public List<string> Extras { get; set; } = new();
    public LuxuryHotelDto Hotel { get; set; } = null!;
    public HotelRoomDto Room { get; set; } = null!;
    public UserDto User { get; set; } = null!;
    public bool CanBeCancelled { get; set; }
}

public class CreateHotelBookingDto
{
    public Guid HotelId { get; set; }
    public Guid RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int Guests { get; set; }
    public string? SpecialRequests { get; set; }
    public bool IncludesBreakfast { get; set; } = false;
    public bool IncludesSpa { get; set; } = false;
    public bool IncludesTransfer { get; set; } = false;
    public List<string> Extras { get; set; } = new();
}