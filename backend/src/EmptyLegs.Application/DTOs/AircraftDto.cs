using EmptyLegs.Core.Enums;

namespace EmptyLegs.Application.DTOs;

public class AircraftDto
{
    public Guid Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Registration { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public AircraftType Type { get; set; }
    public int YearManufactured { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal? CruiseSpeed { get; set; }
    public decimal? Range { get; set; }
    public bool IsActive { get; set; }
    public List<string> Amenities { get; set; } = new();
    public List<string> PhotoUrls { get; set; } = new();
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
}

public class CreateAircraftDto
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
    public List<string> Amenities { get; set; } = new();
    public List<string> PhotoUrls { get; set; } = new();
}

public class UpdateAircraftDto
{
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public string? Registration { get; set; }
    public int? Capacity { get; set; }
    public AircraftType? Type { get; set; }
    public int? YearManufactured { get; set; }
    public string? Description { get; set; }
    public decimal? CruiseSpeed { get; set; }
    public decimal? Range { get; set; }
    public bool? IsActive { get; set; }
    public List<string>? Amenities { get; set; }
    public List<string>? PhotoUrls { get; set; }
}