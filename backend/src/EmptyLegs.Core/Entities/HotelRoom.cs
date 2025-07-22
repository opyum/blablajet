using EmptyLegs.Core.Enums;

namespace EmptyLegs.Core.Entities;

public class HotelRoom : BaseEntity
{
    public string RoomNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Suite, Deluxe, Presidential, etc.
    public string Description { get; set; } = string.Empty;
    public decimal Size { get; set; } // Square meters
    public int MaxOccupancy { get; set; }
    public int Beds { get; set; }
    public string BedType { get; set; } = string.Empty;
    public List<string> Amenities { get; set; } = new();
    public List<string> ImageGallery { get; set; } = new();
    public string? ViewType { get; set; } // Ocean, City, Mountain, etc.
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool HasBalcony { get; set; } = false;
    public bool HasKitchen { get; set; } = false;
    
    // Navigation properties
    public Guid HotelId { get; set; }
    public LuxuryHotel Hotel { get; set; } = null!;
    
    public ICollection<HotelBooking> Bookings { get; set; } = new List<HotelBooking>();
}