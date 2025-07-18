using EmptyLegs.Core.Enums;

namespace EmptyLegs.Core.Entities;

public class Booking : BaseEntity
{
    public string BookingReference { get; set; } = string.Empty;
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public int PassengerCount { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal ServiceFees { get; set; }
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public string? SpecialRequests { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Navigation properties
    public Guid FlightId { get; set; }
    public Flight Flight { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<BookingService> AdditionalServices { get; set; } = new List<BookingService>();
    
    // Computed properties
    public decimal TotalAmount => TotalPrice + ServiceFees;
    public bool CanBeCancelled => Status == BookingStatus.Confirmed && Flight.DepartureTime > DateTime.UtcNow.AddHours(24);
}