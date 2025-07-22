using EmptyLegs.Core.Enums;

namespace EmptyLegs.Core.Entities;

public class LuxuryHotel : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int Stars { get; set; } // 5 or 6 stars
    public LuxuryLevel LuxuryLevel { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> Amenities { get; set; } = new();
    public List<string> ImageGallery { get; set; } = new();
    public string? Website { get; set; }
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public decimal AverageRating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;
    
    // Navigation properties
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    
    public ICollection<HotelRoom> Rooms { get; set; } = new List<HotelRoom>();
    public ICollection<HotelBooking> Bookings { get; set; } = new List<HotelBooking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}