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
    public decimal TotalAmount => TotalPrice + ServiceFees + AdditionalServices.Sum(s => s.TotalPrice);
    public bool CanBeCancelled => (Status == BookingStatus.Pending || Status == BookingStatus.Confirmed) 
                                  && DateTime.UtcNow.Subtract(BookingDate).TotalHours < 24;

    // Methods
    public string GenerateBookingReference()
    {
        var random = new Random();
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var reference = "EL" + new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        BookingReference = reference;
        return reference;
    }
}