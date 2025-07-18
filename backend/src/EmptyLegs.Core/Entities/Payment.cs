namespace EmptyLegs.Core.Entities;

public class Payment : BaseEntity
{
    public string StripePaymentIntentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime? ProcessedAt { get; set; }
    public string? FailureReason { get; set; }
    public string? RefundReason { get; set; }
    public decimal? RefundAmount { get; set; }
    public DateTime? RefundedAt { get; set; }
    
    // Navigation properties
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    
    // Computed properties
    public bool IsSuccessful => Status == "succeeded";
    public bool IsRefunded => RefundAmount.HasValue && RefundAmount > 0;
}