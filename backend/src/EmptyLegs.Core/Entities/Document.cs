namespace EmptyLegs.Core.Entities;

public class Document : BaseEntity
{
    public string Type { get; set; } = string.Empty; // "passport", "id_card", "visa", etc.
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public bool IsVerified { get; set; } = false;
    public DateTime? VerifiedAt { get; set; }
    public string? VerifiedBy { get; set; }
    
    // Navigation properties
    public Guid PassengerId { get; set; }
    public Passenger Passenger { get; set; } = null!;
}