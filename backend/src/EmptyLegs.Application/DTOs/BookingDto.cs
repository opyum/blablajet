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
}