using EmptyLegs.Core.Enums;
using System.Text.Json;

namespace EmptyLegs.Core.Entities;

public class Aircraft : BaseEntity
{
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Registration { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public AircraftType Type { get; set; }
    public int YearManufactured { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal? CruiseSpeed { get; set; }
    public decimal? Range { get; set; }
    public bool IsActive { get; set; } = true;
    
    // JSON serialized properties
    private string _amenitiesJson = "[]";
    public string AmenitiesJson 
    { 
        get => _amenitiesJson; 
        set => _amenitiesJson = value ?? "[]"; 
    }
    
    private string _photoUrlsJson = "[]";
    public string PhotoUrlsJson 
    { 
        get => _photoUrlsJson; 
        set => _photoUrlsJson = value ?? "[]"; 
    }
    
    // Navigation properties
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
    
    // Helper properties
    public List<string> Amenities
    {
        get => JsonSerializer.Deserialize<List<string>>(AmenitiesJson) ?? new List<string>();
        set => AmenitiesJson = JsonSerializer.Serialize(value);
    }
    
    public List<string> PhotoUrls
    {
        get => JsonSerializer.Deserialize<List<string>>(PhotoUrlsJson) ?? new List<string>();
        set => PhotoUrlsJson = JsonSerializer.Serialize(value);
    }
}