using EmptyLegs.Core.Enums;

namespace EmptyLegs.Core.Entities;

public class Yacht : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Length { get; set; } // In meters
    public int MaxGuests { get; set; }
    public int Cabins { get; set; }
    public int Crew { get; set; }
    public YachtType Type { get; set; }
    public LuxuryLevel LuxuryLevel { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<string> Amenities { get; set; } = new();
    public List<string> ImageGallery { get; set; } = new();
    public string HomePort { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsAvailable { get; set; } = true;
    public decimal DailyRate { get; set; }
    public decimal WeeklyRate { get; set; }
    public string? SpecialFeatures { get; set; }
    
    // Navigation properties
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    
    public ICollection<YachtBooking> Bookings { get; set; } = new List<YachtBooking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}