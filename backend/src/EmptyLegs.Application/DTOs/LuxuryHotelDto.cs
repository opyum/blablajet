using EmptyLegs.Core.Enums;

namespace EmptyLegs.Application.DTOs;

public class LuxuryHotelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int Stars { get; set; }
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
    public bool IsActive { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public CompanyDto Company { get; set; } = null!;
    public List<HotelRoomDto> Rooms { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class HotelRoomDto
{
    public Guid Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Size { get; set; }
    public int MaxOccupancy { get; set; }
    public int Beds { get; set; }
    public string BedType { get; set; } = string.Empty;
    public List<string> Amenities { get; set; } = new();
    public List<string> ImageGallery { get; set; } = new();
    public string? ViewType { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; }
    public bool HasBalcony { get; set; }
    public bool HasKitchen { get; set; }
    public Guid HotelId { get; set; }
}

public class CreateLuxuryHotelDto
{
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int Stars { get; set; }
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
}

public class CreateHotelRoomDto
{
    public string RoomNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Size { get; set; }
    public int MaxOccupancy { get; set; }
    public int Beds { get; set; }
    public string BedType { get; set; } = string.Empty;
    public List<string> Amenities { get; set; } = new();
    public List<string> ImageGallery { get; set; } = new();
    public string? ViewType { get; set; }
    public decimal PricePerNight { get; set; }
    public bool HasBalcony { get; set; } = false;
    public bool HasKitchen { get; set; } = false;
}

public class UpdateLuxuryHotelDto
{
    public string? Name { get; set; }
    public string? Brand { get; set; }
    public int? Stars { get; set; }
    public LuxuryLevel? LuxuryLevel { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Amenities { get; set; }
    public List<string>? ImageGallery { get; set; }
    public string? Website { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public bool? IsActive { get; set; }
}

public class LuxuryHotelSearchDto
{
    public string? Location { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int? Guests { get; set; }
    public int? MinStars { get; set; }
    public LuxuryLevel? LuxuryLevel { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinPrice { get; set; }
    public List<string>? Amenities { get; set; }
    public string? RoomType { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "AverageRating";
    public bool SortDescending { get; set; } = true;
}

public class LuxuryHotelSearchResultDto
{
    public List<LuxuryHotelDto> Hotels { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}