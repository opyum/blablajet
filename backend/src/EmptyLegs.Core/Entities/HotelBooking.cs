using EmptyLegs.Core.Enums;

namespace EmptyLegs.Core.Entities;

public class HotelBooking : BaseEntity
{
    public string BookingReference { get; set; } = string.Empty;
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int Nights => (CheckOutDate - CheckInDate).Days;
    public int Guests { get; set; }
    public decimal RoomRate { get; set; }
    public decimal ServiceFees { get; set; }
    public decimal AdditionalFees { get; set; }
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public string? SpecialRequests { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public bool IncludesBreakfast { get; set; } = false;
    public bool IncludesSpa { get; set; } = false;
    public bool IncludesTransfer { get; set; } = false;
    public List<string> Extras { get; set; } = new();
    
    // Navigation properties
    public Guid HotelId { get; set; }
    public LuxuryHotel Hotel { get; set; } = null!;
    
    public Guid RoomId { get; set; }
    public HotelRoom Room { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    
    // Computed properties
    public decimal TotalAmount => (RoomRate * Nights) + ServiceFees + AdditionalFees;
    public bool CanBeCancelled => (Status == BookingStatus.Pending || Status == BookingStatus.Confirmed) 
                                  && DateTime.UtcNow < CheckInDate.AddDays(-1);

    // Methods
    public string GenerateBookingReference()
    {
        var random = new Random();
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var reference = "HT" + new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        BookingReference = reference;
        return reference;
    }
}