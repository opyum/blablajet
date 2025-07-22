using EmptyLegs.Core.Enums;

namespace EmptyLegs.Core.Entities;

public class LuxuryCar : BaseEntity
{
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public CarType Type { get; set; }
    public LuxuryLevel LuxuryLevel { get; set; }
    public string Color { get; set; } = string.Empty;
    public int Seats { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<string> Features { get; set; } = new();
    public List<string> ImageGallery { get; set; } = new();
    public string Location { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsAvailable { get; set; } = true;
    public decimal HourlyRate { get; set; }
    public decimal DailyRate { get; set; }
    public bool WithDriver { get; set; } = true;
    public string? LicenseRequired { get; set; }
    public int MinRentalHours { get; set; } = 4;
    
    // Navigation properties
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    
    public ICollection<CarBooking> Bookings { get; set; } = new List<CarBooking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}