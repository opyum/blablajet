using EmptyLegs.Core.Enums;

namespace EmptyLegs.Application.DTOs;

public class FlightDto
{
    public Guid Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal BasePrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public int AvailableSeats { get; set; }
    public int TotalSeats { get; set; }
    public FlightStatus Status { get; set; }
    public string? Description { get; set; }
    public string? SpecialInstructions { get; set; }
    public bool AllowsAutomaticPricing { get; set; }
    public decimal? MinimumPrice { get; set; }
    
    public AirportDto DepartureAirport { get; set; } = null!;
    public AirportDto ArrivalAirport { get; set; } = null!;
    public AircraftDto Aircraft { get; set; } = null!;
    public CompanyDto Company { get; set; } = null!;
    
    public int BookedSeats { get; set; }
    public decimal OccupancyRate { get; set; }
    public TimeSpan Duration { get; set; }
    public bool IsFullyBooked { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateFlightDto
{
    public string FlightNumber { get; set; } = string.Empty;
    public Guid DepartureAirportId { get; set; }
    public Guid ArrivalAirportId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public Guid AircraftId { get; set; }
    public decimal BasePrice { get; set; }
    public int AvailableSeats { get; set; }
    public string? Description { get; set; }
    public string? SpecialInstructions { get; set; }
    public bool AllowsAutomaticPricing { get; set; } = true;
    public decimal? MinimumPrice { get; set; }
}

public class UpdateFlightDto
{
    public string? FlightNumber { get; set; }
    public DateTime? DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public decimal? BasePrice { get; set; }
    public int? AvailableSeats { get; set; }
    public FlightStatus? Status { get; set; }
    public string? Description { get; set; }
    public string? SpecialInstructions { get; set; }
    public bool? AllowsAutomaticPricing { get; set; }
    public decimal? MinimumPrice { get; set; }
}

public class FlightSearchDto
{
    public string? DepartureAirportCode { get; set; }
    public string? ArrivalAirportCode { get; set; }
    public DateTime? DepartureDate { get; set; }
    public DateTime? DepartureDateFrom { get; set; }
    public DateTime? DepartureDateTo { get; set; }
    public int? PassengerCount { get; set; }
    public AircraftType? AircraftType { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinPrice { get; set; }
    public List<string>? Services { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "DepartureTime";
    public bool SortDescending { get; set; } = false;
}

public class FlightSearchResultDto
{
    public List<FlightDto> Flights { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}

public class FlightAvailabilityDto
{
    public Guid FlightId { get; set; }
    public int AvailableSeats { get; set; }
    public int TotalSeats { get; set; }
    public decimal OccupancyRate { get; set; }
    public bool IsFullyBooked { get; set; }
    public DateTime LastUpdated { get; set; }
}