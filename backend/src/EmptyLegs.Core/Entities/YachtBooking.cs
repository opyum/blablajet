using EmptyLegs.Core.Enums;

namespace EmptyLegs.Core.Entities;

public class YachtBooking : BaseEntity
{
    public string BookingReference { get; set; } = string.Empty;
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Duration => (EndDate - StartDate).Days;
    public int GuestCount { get; set; }
    public decimal BasePrice { get; set; }
    public decimal ServiceFees { get; set; }
    public decimal AdditionalFees { get; set; }
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public string? SpecialRequests { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string ItineraryNotes { get; set; } = string.Empty;
    public bool IncludesCrew { get; set; } = true;
    public bool IncludesFuel { get; set; } = false;
    public List<string> Extras { get; set; } = new();
    
    // Navigation properties
    public Guid YachtId { get; set; }
    public Yacht Yacht { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    
    // Computed properties
    public decimal TotalAmount => BasePrice + ServiceFees + AdditionalFees;
    public bool CanBeCancelled => (Status == BookingStatus.Pending || Status == BookingStatus.Confirmed) 
                                  && DateTime.UtcNow < StartDate.AddDays(-7);

    // Methods
    public string GenerateBookingReference()
    {
        var random = new Random();
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var reference = "YT" + new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        BookingReference = reference;
        return reference;
    }
}