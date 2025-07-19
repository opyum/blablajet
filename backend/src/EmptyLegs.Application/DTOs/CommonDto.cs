namespace EmptyLegs.Application.DTOs;

// Passenger DTOs
public class PassengerDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? PassportNumber { get; set; }
    public string? Nationality { get; set; }
    public string? SpecialRequests { get; set; }
    public Guid BookingId { get; set; }
    public List<DocumentDto> Documents { get; set; } = new();
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class CreatePassengerDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? PassportNumber { get; set; }
    public string? Nationality { get; set; }
    public string? SpecialRequests { get; set; }
}

// Payment DTOs
public class PaymentDto
{
    public Guid Id { get; set; }
    public string StripePaymentIntentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime? ProcessedAt { get; set; }
    public string? FailureReason { get; set; }
    public string? RefundReason { get; set; }
    public decimal? RefundAmount { get; set; }
    public DateTime? RefundedAt { get; set; }
    public Guid BookingId { get; set; }
    public bool IsSuccessful { get; set; }
    public bool IsRefunded { get; set; }
}

public class CreatePaymentDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public string PaymentMethod { get; set; } = string.Empty;
    public Guid BookingId { get; set; }
    public string? PaymentMethodId { get; set; } // Stripe payment method ID
}

// Review DTOs
public class ReviewDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsVisible { get; set; }
    public Guid UserId { get; set; }
    public UserDto User { get; set; } = null!;
    public Guid? FlightId { get; set; }
    public Guid? CompanyId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewDto
{
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public Guid? FlightId { get; set; }
    public Guid? CompanyId { get; set; }
}

// UserAlert DTOs
public class UserAlertDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DepartureAirportCode { get; set; }
    public string? ArrivalAirportCode { get; set; }
    public DateTime? DepartureDateFrom { get; set; }
    public DateTime? DepartureDateTo { get; set; }
    public int? MinPassengers { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool IsActive { get; set; }
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool SmsNotifications { get; set; }
    public Guid UserId { get; set; }
}

public class CreateUserAlertDto
{
    public string Name { get; set; } = string.Empty;
    public string? DepartureAirportCode { get; set; }
    public string? ArrivalAirportCode { get; set; }
    public DateTime? DepartureDateFrom { get; set; }
    public DateTime? DepartureDateTo { get; set; }
    public int? MinPassengers { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; } = false;
}

// Document DTOs
public class DocumentDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerifiedBy { get; set; }
    public Guid PassengerId { get; set; }
}

// BookingService DTOs
public class BookingServiceDto
{
    public Guid Id { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public Guid BookingId { get; set; }
    public decimal TotalPrice { get; set; }
}

public class CreateBookingServiceDto
{
    public string ServiceType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
}