namespace EmptyLegs.Core.Entities;

public class BookingService : BaseEntity
{
    public string ServiceType { get; set; } = string.Empty; // "transfer", "catering", "wifi", etc.
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
    
    // Navigation properties
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    
    // Computed properties
    public decimal TotalPrice => Price * Quantity;
}