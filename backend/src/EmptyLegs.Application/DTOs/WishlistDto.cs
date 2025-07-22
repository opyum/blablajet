using EmptyLegs.Core.Enums;

namespace EmptyLegs.Application.DTOs;

public class WishlistDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public Guid UserId { get; set; }
    public List<WishlistItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class WishlistItemDto
{
    public Guid Id { get; set; }
    public VehicleType VehicleType { get; set; }
    public Guid VehicleId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Populated based on VehicleType and VehicleId
    public object? VehicleDetails { get; set; }
}

public class CreateWishlistDto
{
    public string Name { get; set; } = "Ma liste de souhaits";
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;
}

public class UpdateWishlistDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsPublic { get; set; }
}

public class AddToWishlistDto
{
    public Guid WishlistId { get; set; }
    public VehicleType VehicleType { get; set; }
    public Guid VehicleId { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class RemoveFromWishlistDto
{
    public Guid WishlistId { get; set; }
    public Guid ItemId { get; set; }
}