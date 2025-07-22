using EmptyLegs.Core.Enums;

namespace EmptyLegs.Core.Entities;

public class Wishlist : BaseEntity
{
    public string Name { get; set; } = "Ma liste de souhaits";
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;
    
    // Navigation properties
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
}

public class WishlistItem : BaseEntity
{
    public VehicleType VehicleType { get; set; }
    public Guid VehicleId { get; set; }
    public string Notes { get; set; } = string.Empty;
    
    // Navigation properties
    public Guid WishlistId { get; set; }
    public Wishlist Wishlist { get; set; } = null!;
}