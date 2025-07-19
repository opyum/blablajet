using EmptyLegs.Application.DTOs;
using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Enums;
using EmptyLegs.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace EmptyLegs.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FlightsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<FlightsController> _logger;

    public FlightsController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<FlightsController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Search for available flights
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<FlightSearchResultDto>> SearchFlights([FromQuery] FlightSearchDto searchDto)
    {
        try
        {
            _logger.LogInformation("Searching flights with criteria: {@SearchCriteria}", searchDto);

            // Build query
            var query = await _unitOfWork.Flights.GetAllAsync();
            
            // Apply filters
            if (!string.IsNullOrEmpty(searchDto.DepartureAirportCode))
            {
                query = query.Where(f => f.DepartureAirport.IataCode == searchDto.DepartureAirportCode);
            }
            
            if (!string.IsNullOrEmpty(searchDto.ArrivalAirportCode))
            {
                query = query.Where(f => f.ArrivalAirport.IataCode == searchDto.ArrivalAirportCode);
            }
            
            if (searchDto.DepartureDate.HasValue)
            {
                var date = searchDto.DepartureDate.Value.Date;
                query = query.Where(f => f.DepartureTime.Date == date);
            }
            
            if (searchDto.DepartureDateFrom.HasValue && searchDto.DepartureDateTo.HasValue)
            {
                query = query.Where(f => f.DepartureTime >= searchDto.DepartureDateFrom.Value 
                                      && f.DepartureTime <= searchDto.DepartureDateTo.Value);
            }
            
            if (searchDto.PassengerCount.HasValue)
            {
                query = query.Where(f => f.AvailableSeats >= searchDto.PassengerCount.Value);
            }
            
            if (searchDto.AircraftType.HasValue)
            {
                query = query.Where(f => f.Aircraft.Type == searchDto.AircraftType.Value);
            }
            
            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(f => f.CurrentPrice <= searchDto.MaxPrice.Value);
            }
            
            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(f => f.CurrentPrice >= searchDto.MinPrice.Value);
            }

            // Only show available flights
            query = query.Where(f => f.Status == FlightStatus.Available && f.DepartureTime > DateTime.UtcNow);

            // Count total results
            var totalCount = query.Count();

            // Apply sorting
            query = searchDto.SortBy?.ToLower() switch
            {
                "price" => searchDto.SortDescending 
                    ? query.OrderByDescending(f => f.CurrentPrice)
                    : query.OrderBy(f => f.CurrentPrice),
                "duration" => searchDto.SortDescending
                    ? query.OrderByDescending(f => f.ArrivalTime.Subtract(f.DepartureTime))
                    : query.OrderBy(f => f.ArrivalTime.Subtract(f.DepartureTime)),
                "departuretime" or _ => searchDto.SortDescending
                    ? query.OrderByDescending(f => f.DepartureTime)
                    : query.OrderBy(f => f.DepartureTime)
            };

            // Apply pagination
            var flights = query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToList();

            var flightDtos = _mapper.Map<List<FlightDto>>(flights);
            var totalPages = (int)Math.Ceiling(totalCount / (double)searchDto.PageSize);

            var result = new FlightSearchResultDto
            {
                Flights = flightDtos,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = totalPages,
                HasNextPage = searchDto.Page < totalPages,
                HasPreviousPage = searchDto.Page > 1
            };

            _logger.LogInformation("Found {FlightCount} flights matching criteria", flightDtos.Count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching flights");
            return StatusCode(500, "An error occurred while searching flights");
        }
    }

    /// <summary>
    /// Get flight by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FlightDto>> GetFlight(Guid id)
    {
        try
        {
            var flight = await _unitOfWork.Flights.FirstOrDefaultAsync(f => f.Id == id);
            
            if (flight == null)
            {
                return NotFound($"Flight with ID {id} not found");
            }

            var flightDto = _mapper.Map<FlightDto>(flight);
            return Ok(flightDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving flight {FlightId}", id);
            return StatusCode(500, "An error occurred while retrieving the flight");
        }
    }

    /// <summary>
    /// Create a new flight (Company only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Company")]
    public async Task<ActionResult<FlightDto>> CreateFlight([FromBody] CreateFlightDto createFlightDto)
    {
        try
        {
            // Validate that the company exists and user belongs to it
            var userId = GetCurrentUserId();
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            
            if (user?.CompanyId == null)
            {
                return Forbid("User must be associated with a company to create flights");
            }

            // Validate airports exist
            var departureAirport = await _unitOfWork.Airports.GetByIdAsync(createFlightDto.DepartureAirportId);
            var arrivalAirport = await _unitOfWork.Airports.GetByIdAsync(createFlightDto.ArrivalAirportId);
            
            if (departureAirport == null || arrivalAirport == null)
            {
                return BadRequest("Invalid departure or arrival airport");
            }

            // Validate aircraft belongs to company
            var aircraft = await _unitOfWork.Aircraft.FirstOrDefaultAsync(a => 
                a.Id == createFlightDto.AircraftId && a.CompanyId == user.CompanyId);
            
            if (aircraft == null)
            {
                return BadRequest("Aircraft not found or doesn't belong to your company");
            }

            // Create flight entity
            var flight = new Flight
            {
                FlightNumber = createFlightDto.FlightNumber,
                DepartureAirportId = createFlightDto.DepartureAirportId,
                ArrivalAirportId = createFlightDto.ArrivalAirportId,
                DepartureTime = createFlightDto.DepartureTime,
                ArrivalTime = createFlightDto.ArrivalTime ?? CalculateArrivalTime(createFlightDto.DepartureTime, departureAirport, arrivalAirport),
                AircraftId = createFlightDto.AircraftId,
                BasePrice = createFlightDto.BasePrice,
                CurrentPrice = createFlightDto.BasePrice, // Initial price equals base price
                AvailableSeats = createFlightDto.AvailableSeats,
                TotalSeats = createFlightDto.AvailableSeats,
                Description = createFlightDto.Description,
                SpecialInstructions = createFlightDto.SpecialInstructions,
                AllowsAutomaticPricing = createFlightDto.AllowsAutomaticPricing,
                MinimumPrice = createFlightDto.MinimumPrice,
                CompanyId = user.CompanyId.Value,
                Status = FlightStatus.Available
            };

            await _unitOfWork.Flights.AddAsync(flight);
            await _unitOfWork.SaveChangesAsync();

            // Return the created flight
            var createdFlight = await _unitOfWork.Flights.FirstOrDefaultAsync(f => f.Id == flight.Id);
            var flightDto = _mapper.Map<FlightDto>(createdFlight);

            _logger.LogInformation("Flight {FlightNumber} created successfully by company {CompanyId}", 
                flight.FlightNumber, user.CompanyId);

            return CreatedAtAction(nameof(GetFlight), new { id = flight.Id }, flightDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating flight");
            return StatusCode(500, "An error occurred while creating the flight");
        }
    }

    /// <summary>
    /// Update flight (Company only)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Company")]
    public async Task<ActionResult<FlightDto>> UpdateFlight(Guid id, [FromBody] UpdateFlightDto updateFlightDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            
            var flight = await _unitOfWork.Flights.FirstOrDefaultAsync(f => f.Id == id);
            
            if (flight == null)
            {
                return NotFound($"Flight with ID {id} not found");
            }

            // Verify user owns this flight
            if (flight.CompanyId != user?.CompanyId)
            {
                return Forbid("You can only update flights belonging to your company");
            }

            // Update properties
            if (!string.IsNullOrEmpty(updateFlightDto.FlightNumber))
                flight.FlightNumber = updateFlightDto.FlightNumber;
            
            if (updateFlightDto.DepartureTime.HasValue)
                flight.DepartureTime = updateFlightDto.DepartureTime.Value;
            
            if (updateFlightDto.ArrivalTime.HasValue)
                flight.ArrivalTime = updateFlightDto.ArrivalTime.Value;
            
            if (updateFlightDto.BasePrice.HasValue)
            {
                flight.BasePrice = updateFlightDto.BasePrice.Value;
                flight.CurrentPrice = updateFlightDto.BasePrice.Value; // Reset current price
            }
            
            if (updateFlightDto.AvailableSeats.HasValue)
                flight.AvailableSeats = updateFlightDto.AvailableSeats.Value;
            
            if (updateFlightDto.Status.HasValue)
                flight.Status = updateFlightDto.Status.Value;
            
            if (updateFlightDto.Description != null)
                flight.Description = updateFlightDto.Description;
            
            if (updateFlightDto.SpecialInstructions != null)
                flight.SpecialInstructions = updateFlightDto.SpecialInstructions;
            
            if (updateFlightDto.AllowsAutomaticPricing.HasValue)
                flight.AllowsAutomaticPricing = updateFlightDto.AllowsAutomaticPricing.Value;
            
            if (updateFlightDto.MinimumPrice.HasValue)
                flight.MinimumPrice = updateFlightDto.MinimumPrice.Value;

            _unitOfWork.Flights.Update(flight);
            await _unitOfWork.SaveChangesAsync();

            var updatedFlight = await _unitOfWork.Flights.FirstOrDefaultAsync(f => f.Id == id);
            var flightDto = _mapper.Map<FlightDto>(updatedFlight);

            _logger.LogInformation("Flight {FlightId} updated successfully", id);
            return Ok(flightDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating flight {FlightId}", id);
            return StatusCode(500, "An error occurred while updating the flight");
        }
    }

    /// <summary>
    /// Delete flight (Company only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Company")]
    public async Task<ActionResult> DeleteFlight(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            
            var flight = await _unitOfWork.Flights.FirstOrDefaultAsync(f => f.Id == id);
            
            if (flight == null)
            {
                return NotFound($"Flight with ID {id} not found");
            }

            // Verify user owns this flight
            if (flight.CompanyId != user?.CompanyId)
            {
                return Forbid("You can only delete flights belonging to your company");
            }

            // Check if flight has bookings
            var hasBookings = await _unitOfWork.Bookings.AnyAsync(b => b.FlightId == id);
            if (hasBookings)
            {
                return BadRequest("Cannot delete flight with existing bookings");
            }

            await _unitOfWork.Flights.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Flight {FlightId} deleted successfully", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting flight {FlightId}", id);
            return StatusCode(500, "An error occurred while deleting the flight");
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
        return Guid.Parse(userIdClaim?.Value ?? throw new UnauthorizedAccessException("User ID not found in token"));
    }

    private static DateTime CalculateArrivalTime(DateTime departureTime, Airport departureAirport, Airport arrivalAirport)
    {
        // Simple calculation based on distance (this should be more sophisticated in production)
        var distance = CalculateDistance(
            (double)departureAirport.Latitude, (double)departureAirport.Longitude,
            (double)arrivalAirport.Latitude, (double)arrivalAirport.Longitude);
        
        // Assume average speed of 800 km/h for private jets
        var flightTimeHours = distance / 800.0;
        
        return departureTime.AddHours(flightTimeHours);
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in kilometers
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}