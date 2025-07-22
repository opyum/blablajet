using EmptyLegs.Core.Enums;

namespace EmptyLegs.Core.Entities;

public class CarBooking : BaseEntity
{
    public string BookingReference { get; set; } = string.Empty;
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int Hours => (int)(EndDateTime - StartDateTime).TotalHours;
    public string PickupLocation { get; set; } = string.Empty;
    public string DropoffLocation { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal ServiceFees { get; set; }
    public decimal AdditionalFees { get; set; }
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public string? SpecialRequests { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public bool RequiresDriver { get; set; } = true;
    public string? DriverNotes { get; set; }
    public List<string> Extras { get; set; } = new();
    
    // Navigation properties
    public Guid CarId { get; set; }
    public LuxuryCar Car { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    
    // Computed properties
    public decimal TotalAmount => BasePrice + ServiceFees + AdditionalFees;
    public bool CanBeCancelled => (Status == BookingStatus.Pending || Status == BookingStatus.Confirmed) 
                                  && DateTime.UtcNow < StartDateTime.AddHours(-4);

    // Methods
    public string GenerateBookingReference()
    {
        var random = new Random();
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var reference = "CR" + new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        BookingReference = reference;
        return reference;
    }
}