namespace EmptyLegs.Core.Entities;

public class Passenger : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? PassportNumber { get; set; }
    public string? Nationality { get; set; }
    public string? SpecialRequests { get; set; }
    
    // Navigation properties
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    
    // Computed properties
    public string FullName => $"{FirstName} {LastName}";
    public int Age => DateTime.UtcNow.Year - DateOfBirth.Year - (DateTime.UtcNow.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
}