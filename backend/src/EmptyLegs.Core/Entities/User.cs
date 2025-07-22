using EmptyLegs.Core.Enums;

namespace EmptyLegs.Core.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public UserRole Role { get; set; } = UserRole.Customer;
    public bool IsActive { get; set; } = true;
    public bool IsEmailVerified { get; set; } = false;
    public string? PasswordHash { get; set; }
    
    // Navigation properties
    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<YachtBooking> YachtBookings { get; set; } = new List<YachtBooking>();
    public ICollection<CarBooking> CarBookings { get; set; } = new List<CarBooking>();
    public ICollection<HotelBooking> HotelBookings { get; set; } = new List<HotelBooking>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public ICollection<UserAlert> Alerts { get; set; } = new List<UserAlert>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    
    public string FullName => $"{FirstName} {LastName}";
}