namespace EmptyLegs.Core.Entities;

public class Review : BaseEntity
{
    public int Rating { get; set; } // 1-5 stars
    public string Title { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public bool IsVerified { get; set; } = false;
    public bool IsVisible { get; set; } = true;
    
    // Navigation properties
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public Guid? FlightId { get; set; }
    public Flight? Flight { get; set; }
    
    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }
}