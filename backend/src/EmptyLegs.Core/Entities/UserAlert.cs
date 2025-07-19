namespace EmptyLegs.Core.Entities;

public class UserAlert : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? DepartureAirportCode { get; set; }
    public string? ArrivalAirportCode { get; set; }
    public DateTime? DepartureDateFrom { get; set; }
    public DateTime? DepartureDateTo { get; set; }
    public int? MinPassengers { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool IsActive { get; set; } = true;
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; } = false;
    
    // Navigation properties
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}