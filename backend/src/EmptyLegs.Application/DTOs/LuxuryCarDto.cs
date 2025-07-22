using EmptyLegs.Core.Enums;

namespace EmptyLegs.Application.DTOs;

public class LuxuryCarDto
{
    public Guid Id { get; set; }
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
    public bool IsAvailable { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal DailyRate { get; set; }
    public bool WithDriver { get; set; }
    public string? LicenseRequired { get; set; }
    public int MinRentalHours { get; set; }
    public CompanyDto Company { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateLuxuryCarDto
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
    public decimal HourlyRate { get; set; }
    public decimal DailyRate { get; set; }
    public bool WithDriver { get; set; } = true;
    public string? LicenseRequired { get; set; }
    public int MinRentalHours { get; set; } = 4;
}

public class UpdateLuxuryCarDto
{
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public CarType? Type { get; set; }
    public LuxuryLevel? LuxuryLevel { get; set; }
    public string? Color { get; set; }
    public int? Seats { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Features { get; set; }
    public List<string>? ImageGallery { get; set; }
    public string? Location { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool? IsAvailable { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal? DailyRate { get; set; }
    public bool? WithDriver { get; set; }
    public string? LicenseRequired { get; set; }
    public int? MinRentalHours { get; set; }
}

public class LuxuryCarSearchDto
{
    public string? Location { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public CarType? Type { get; set; }
    public LuxuryLevel? LuxuryLevel { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinPrice { get; set; }
    public bool? WithDriver { get; set; }
    public int? MinSeats { get; set; }
    public List<string>? Features { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "HourlyRate";
    public bool SortDescending { get; set; } = false;
}

public class LuxuryCarSearchResultDto
{
    public List<LuxuryCarDto> Cars { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}