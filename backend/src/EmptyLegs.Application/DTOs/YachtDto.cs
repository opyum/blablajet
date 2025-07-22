using EmptyLegs.Core.Enums;

namespace EmptyLegs.Application.DTOs;

public class YachtDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Length { get; set; }
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
    public bool IsAvailable { get; set; }
    public decimal DailyRate { get; set; }
    public decimal WeeklyRate { get; set; }
    public string? SpecialFeatures { get; set; }
    public CompanyDto Company { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateYachtDto
{
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Length { get; set; }
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
    public decimal DailyRate { get; set; }
    public decimal WeeklyRate { get; set; }
    public string? SpecialFeatures { get; set; }
}

public class UpdateYachtDto
{
    public string? Name { get; set; }
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public int? Year { get; set; }
    public decimal? Length { get; set; }
    public int? MaxGuests { get; set; }
    public int? Cabins { get; set; }
    public int? Crew { get; set; }
    public YachtType? Type { get; set; }
    public LuxuryLevel? LuxuryLevel { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Amenities { get; set; }
    public List<string>? ImageGallery { get; set; }
    public string? HomePort { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool? IsAvailable { get; set; }
    public decimal? DailyRate { get; set; }
    public decimal? WeeklyRate { get; set; }
    public string? SpecialFeatures { get; set; }
}

public class YachtSearchDto
{
    public string? Location { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? MaxGuests { get; set; }
    public YachtType? Type { get; set; }
    public LuxuryLevel? LuxuryLevel { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MinLength { get; set; }
    public decimal? MaxLength { get; set; }
    public List<string>? Amenities { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "DailyRate";
    public bool SortDescending { get; set; } = false;
}

public class YachtSearchResultDto
{
    public List<YachtDto> Yachts { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}