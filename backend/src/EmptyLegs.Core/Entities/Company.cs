namespace EmptyLegs.Core.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string License { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Website { get; set; }
    public bool IsVerified { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public decimal AverageRating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;
    
    // Navigation properties
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Aircraft> Aircraft { get; set; } = new List<Aircraft>();
    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}