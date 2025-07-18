using AutoMapper;
using EmptyLegs.Application.DTOs;
using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Enums;
using EmptyLegs.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmptyLegs.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BookingsController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all bookings (Admin only) or user's own bookings
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="status">Filter by booking status</param>
    /// <param name="userId">Filter by user ID (Admin only)</param>
    /// <returns>Paginated list of bookings</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] BookingStatus? status = null,
        [FromQuery] Guid? userId = null)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            IEnumerable<Booking> bookings;

            if (currentUserRole == "Admin")
            {
                // Admin can see all bookings
                if (userId.HasValue || status.HasValue)
                {
                    bookings = await _unitOfWork.Bookings.FindAsync(b =>
                        (!userId.HasValue || b.UserId == userId.Value) &&
                        (!status.HasValue || b.Status == status.Value));
                }
                else
                {
                    bookings = await _unitOfWork.Bookings.GetPagedAsync(page, pageSize);
                }
            }
            else
            {
                // Regular users can only see their own bookings
                bookings = status.HasValue
                    ? await _unitOfWork.Bookings.FindAsync(b => b.UserId == currentUserId && b.Status == status.Value)
                    : await _unitOfWork.Bookings.FindAsync(b => b.UserId == currentUserId);
            }

            var bookingDtos = _mapper.Map<IEnumerable<BookingDto>>(bookings);
            
            _logger.LogInformation("Retrieved {Count} bookings for user {UserId}", bookingDtos.Count(), currentUserId);
            return Ok(bookingDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings");
            return StatusCode(500, new { message = "An error occurred while retrieving bookings" });
        }
    }

    /// <summary>
    /// Get booking by ID
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <returns>Booking details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<BookingDto>> GetBooking(Guid id)
    {
        try
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            // Check authorization - users can only access their own bookings
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (currentUserRole != "Admin" && booking.UserId != currentUserId)
            {
                return Forbid("You can only access your own bookings");
            }

            var bookingDto = _mapper.Map<BookingDto>(booking);
            return Ok(bookingDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking {BookingId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the booking" });
        }
    }

    /// <summary>
    /// Create new booking
    /// </summary>
    /// <param name="createBookingDto">Booking data</param>
    /// <returns>Created booking details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] CreateBookingDto createBookingDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate flight exists and is available
            var flight = await _unitOfWork.Flights.GetByIdAsync(createBookingDto.FlightId);
            if (flight == null)
            {
                return NotFound(new { message = "Flight not found" });
            }

            if (flight.Status != FlightStatus.Available)
            {
                return BadRequest(new { message = "Flight is not available for booking" });
            }

            // Check available seats
            var existingBookings = await _unitOfWork.Bookings.FindAsync(b => 
                b.FlightId == createBookingDto.FlightId && 
                (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending));
            
            var bookedSeats = existingBookings.Sum(b => b.PassengerCount);
            
            if (bookedSeats + createBookingDto.PassengerCount > flight.AvailableSeats)
            {
                return Conflict(new { 
                    message = "Not enough available seats",
                    availableSeats = flight.AvailableSeats - bookedSeats,
                    requestedSeats = createBookingDto.PassengerCount
                });
            }

            // Validate passengers count matches
            if (createBookingDto.Passengers.Count != createBookingDto.PassengerCount)
            {
                return BadRequest(new { message = "Number of passengers does not match passenger count" });
            }

            // Create booking
            var booking = new Booking
            {
                FlightId = createBookingDto.FlightId,
                UserId = GetCurrentUserId(),
                PassengerCount = createBookingDto.PassengerCount,
                TotalPrice = CalculateBookingPrice(flight, createBookingDto.PassengerCount),
                ServiceFees = CalculateServiceFees(flight, createBookingDto.PassengerCount),
                Status = BookingStatus.Pending,
                BookingDate = DateTime.UtcNow,
                SpecialRequests = createBookingDto.SpecialRequests
            };

            // Generate booking reference
            booking.GenerateBookingReference();

            await _unitOfWork.Bookings.AddAsync(booking);

            // Add passengers
            foreach (var passengerDto in createBookingDto.Passengers)
            {
                var passenger = _mapper.Map<Passenger>(passengerDto);
                passenger.BookingId = booking.Id;
                await _unitOfWork.Passengers.AddAsync(passenger);
            }

            await _unitOfWork.SaveChangesAsync();

            var bookingDto = _mapper.Map<BookingDto>(booking);
            
            _logger.LogInformation("Booking {BookingId} created successfully for flight {FlightId}", booking.Id, createBookingDto.FlightId);
            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, bookingDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            return StatusCode(500, new { message = "An error occurred while creating the booking" });
        }
    }

    /// <summary>
    /// Update booking status
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="updateStatusDto">New status</param>
    /// <returns>Updated booking details</returns>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<BookingDto>> UpdateBookingStatus(Guid id, [FromBody] UpdateBookingStatusDto updateStatusDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            // Check authorization
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            // Only admin or company users can change status to confirmed, completed
            // Regular users can only cancel their own bookings
            if (currentUserRole != "Admin" && currentUserRole != "Company")
            {
                if (booking.UserId != currentUserId || updateStatusDto.Status != BookingStatus.Cancelled)
                {
                    return Forbid("You can only cancel your own bookings");
                }
            }

            // Validate status transition
            if (!IsValidStatusTransition(booking.Status, updateStatusDto.Status))
            {
                return BadRequest(new { 
                    message = "Invalid status transition",
                    currentStatus = booking.Status.ToString(),
                    requestedStatus = updateStatusDto.Status.ToString()
                });
            }

            booking.Status = updateStatusDto.Status;
            booking.UpdatedAt = DateTime.UtcNow;

            if (updateStatusDto.Status == BookingStatus.Cancelled)
            {
                booking.CancellationReason = updateStatusDto.Reason;
                booking.CancelledAt = DateTime.UtcNow;
            }

            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            var bookingDto = _mapper.Map<BookingDto>(booking);
            
            _logger.LogInformation("Booking {BookingId} status updated to {Status}", id, updateStatusDto.Status);
            return Ok(bookingDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking status {BookingId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the booking status" });
        }
    }

    /// <summary>
    /// Cancel booking
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="reason">Cancellation reason</param>
    /// <returns>Success status</returns>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CancelBooking(Guid id, [FromBody] string? reason = null)
    {
        try
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            // Check authorization - users can cancel their own bookings
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (currentUserRole != "Admin" && booking.UserId != currentUserId)
            {
                return Forbid("You can only cancel your own bookings");
            }

            // Check if booking can be cancelled
            if (!booking.CanBeCancelled)
            {
                return BadRequest(new { message = "This booking cannot be cancelled" });
            }

            booking.Status = BookingStatus.Cancelled;
            booking.CancellationReason = reason;
            booking.CancelledAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Booking {BookingId} cancelled by user {UserId}", id, currentUserId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
            return StatusCode(500, new { message = "An error occurred while cancelling the booking" });
        }
    }

    /// <summary>
    /// Confirm booking (Company/Admin only)
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <returns>Success status</returns>
    [HttpPost("{id:guid}/confirm")]
    [Authorize(Roles = "Company,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ConfirmBooking(Guid id)
    {
        try
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            if (booking.Status != BookingStatus.Pending)
            {
                return BadRequest(new { message = "Only pending bookings can be confirmed" });
            }

            booking.Status = BookingStatus.Confirmed;
            booking.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Booking {BookingId} confirmed", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming booking {BookingId}", id);
            return StatusCode(500, new { message = "An error occurred while confirming the booking" });
        }
    }

    /// <summary>
    /// Complete booking (Company/Admin only)
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <returns>Success status</returns>
    [HttpPost("{id:guid}/complete")]
    [Authorize(Roles = "Company,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CompleteBooking(Guid id)
    {
        try
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            if (booking.Status != BookingStatus.Confirmed)
            {
                return BadRequest(new { message = "Only confirmed bookings can be completed" });
            }

            booking.Status = BookingStatus.Completed;
            booking.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Booking {BookingId} completed", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing booking {BookingId}", id);
            return StatusCode(500, new { message = "An error occurred while completing the booking" });
        }
    }

    /// <summary>
    /// Get booking passengers
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <returns>List of passengers</returns>
    [HttpGet("{id:guid}/passengers")]
    [ProducesResponseType(typeof(IEnumerable<PassengerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<PassengerDto>>> GetBookingPassengers(Guid id)
    {
        try
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            // Check authorization
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (currentUserRole != "Admin" && booking.UserId != currentUserId)
            {
                return Forbid("You can only access your own booking passengers");
            }

            var passengers = await _unitOfWork.Passengers.FindAsync(p => p.BookingId == id);
            var passengerDtos = _mapper.Map<IEnumerable<PassengerDto>>(passengers);

            return Ok(passengerDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving passengers for booking {BookingId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving booking passengers" });
        }
    }

    /// <summary>
    /// Get booking payments
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <returns>List of payments</returns>
    [HttpGet("{id:guid}/payments")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetBookingPayments(Guid id)
    {
        try
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            // Check authorization
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (currentUserRole != "Admin" && booking.UserId != currentUserId)
            {
                return Forbid("You can only access your own booking payments");
            }

            var payments = await _unitOfWork.Payments.FindAsync(p => p.BookingId == id);
            var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(payments);

            return Ok(paymentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments for booking {BookingId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving booking payments" });
        }
    }

    /// <summary>
    /// Get booking statistics (Admin only)
    /// </summary>
    /// <returns>Booking statistics</returns>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<object>> GetBookingStatistics()
    {
        try
        {
            var totalBookings = await _unitOfWork.Bookings.CountAsync();
            var pendingBookings = await _unitOfWork.Bookings.CountAsync(b => b.Status == BookingStatus.Pending);
            var confirmedBookings = await _unitOfWork.Bookings.CountAsync(b => b.Status == BookingStatus.Confirmed);
            var completedBookings = await _unitOfWork.Bookings.CountAsync(b => b.Status == BookingStatus.Completed);
            var cancelledBookings = await _unitOfWork.Bookings.CountAsync(b => b.Status == BookingStatus.Cancelled);

            var statistics = new
            {
                Total = totalBookings,
                ByStatus = new
                {
                    Pending = pendingBookings,
                    Confirmed = confirmedBookings,
                    Completed = completedBookings,
                    Cancelled = cancelledBookings
                },
                RecentActivity = new
                {
                    LastDay = await _unitOfWork.Bookings.CountAsync(b => b.BookingDate >= DateTime.UtcNow.AddDays(-1)),
                    LastWeek = await _unitOfWork.Bookings.CountAsync(b => b.BookingDate >= DateTime.UtcNow.AddDays(-7)),
                    LastMonth = await _unitOfWork.Bookings.CountAsync(b => b.BookingDate >= DateTime.UtcNow.AddMonths(-1))
                }
            };

            _logger.LogInformation("Booking statistics retrieved");
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking statistics");
            return StatusCode(500, new { message = "An error occurred while retrieving booking statistics" });
        }
    }

    private decimal CalculateBookingPrice(Flight flight, int passengerCount)
    {
        // Basic price calculation - could be enhanced with dynamic pricing
        return flight.CurrentPrice * passengerCount;
    }

    private decimal CalculateServiceFees(Flight flight, int passengerCount)
    {
        // Service fees (e.g., 3% of base price)
        var basePrice = flight.CurrentPrice * passengerCount;
        return Math.Round(basePrice * 0.03m, 2);
    }

    private static bool IsValidStatusTransition(BookingStatus currentStatus, BookingStatus newStatus)
    {
        return currentStatus switch
        {
            BookingStatus.Pending => newStatus is BookingStatus.Confirmed or BookingStatus.Cancelled,
            BookingStatus.Confirmed => newStatus is BookingStatus.Completed or BookingStatus.Cancelled,
            BookingStatus.Completed => false, // Completed bookings cannot be changed
            BookingStatus.Cancelled => false, // Cancelled bookings cannot be changed
            _ => false
        };
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
}