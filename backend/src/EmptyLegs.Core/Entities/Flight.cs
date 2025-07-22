using EmptyLegs.Core.Enums;

namespace EmptyLegs.Core.Entities;

public class Flight : BaseEntity
{
    public string FlightNumber { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal BasePrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public int AvailableSeats { get; set; }
    public int TotalSeats { get; set; }
    public FlightStatus Status { get; set; } = FlightStatus.Available;
    public string? Description { get; set; }
    public string? SpecialInstructions { get; set; }
    public bool AllowsAutomaticPricing { get; set; } = true;
    public decimal? MinimumPrice { get; set; }
    
    // Navigation properties
    public Guid DepartureAirportId { get; set; }
    public Airport DepartureAirport { get; set; } = null!;
    
    public Guid ArrivalAirportId { get; set; }
    public Airport ArrivalAirport { get; set; } = null!;
    
    public Guid AircraftId { get; set; }
    public Aircraft Aircraft { get; set; } = null!;
    
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    
    // Computed properties
    public int BookedSeats => Bookings?.Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.PaymentConfirmed).Sum(b => b.PassengerCount) ?? 0;
    public int RemainingSeats => Math.Max(0, AvailableSeats - (Bookings?.Sum(b => b.PassengerCount) ?? 0));
    public decimal OccupancyRate => TotalSeats > 0 ? (decimal)BookedSeats / TotalSeats : 0;
    public TimeSpan Duration => ArrivalTime - DepartureTime;
    public bool IsFullyBooked => AvailableSeats > 0 && BookedSeats >= AvailableSeats;
}