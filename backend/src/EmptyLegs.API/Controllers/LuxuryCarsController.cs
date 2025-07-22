using AutoMapper;
using EmptyLegs.Application.DTOs;
using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Enums;
using EmptyLegs.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmptyLegs.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class LuxuryCarsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<LuxuryCarsController> _logger;

    public LuxuryCarsController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<LuxuryCarsController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Search for available luxury cars
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(LuxuryCarSearchResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<LuxuryCarSearchResultDto>> SearchLuxuryCars([FromQuery] LuxuryCarSearchDto searchDto)
    {
        try
        {
            _logger.LogInformation("Searching luxury cars with criteria: {@SearchCriteria}", searchDto);

            var query = await _unitOfWork.LuxuryCars.GetAllAsync();
            
            // Apply filters
            if (!string.IsNullOrEmpty(searchDto.Location))
            {
                query = query.Where(c => c.City.Contains(searchDto.Location, StringComparison.OrdinalIgnoreCase) ||
                                        c.Country.Contains(searchDto.Location, StringComparison.OrdinalIgnoreCase));
            }

            if (searchDto.Type.HasValue)
            {
                query = query.Where(c => c.Type == searchDto.Type.Value);
            }

            if (searchDto.LuxuryLevel.HasValue)
            {
                query = query.Where(c => c.LuxuryLevel == searchDto.LuxuryLevel.Value);
            }

            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(c => c.HourlyRate <= searchDto.MaxPrice.Value);
            }

            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(c => c.HourlyRate >= searchDto.MinPrice.Value);
            }

            if (searchDto.WithDriver.HasValue)
            {
                query = query.Where(c => c.WithDriver == searchDto.WithDriver.Value);
            }

            if (searchDto.MinSeats.HasValue)
            {
                query = query.Where(c => c.Seats >= searchDto.MinSeats.Value);
            }

            // Filter by features
            if (searchDto.Features?.Any() == true)
            {
                foreach (var feature in searchDto.Features)
                {
                    query = query.Where(c => c.Features.Contains(feature));
                }
            }

            // Only show available cars
            query = query.Where(c => c.IsAvailable);

            // Count total results
            var totalCount = query.Count();

            // Apply sorting
            query = searchDto.SortBy?.ToLower() switch
            {
                "hourlyrate" or "price" => searchDto.SortDescending 
                    ? query.OrderByDescending(c => c.HourlyRate)
                    : query.OrderBy(c => c.HourlyRate),
                "dailyrate" => searchDto.SortDescending
                    ? query.OrderByDescending(c => c.DailyRate)
                    : query.OrderBy(c => c.DailyRate),
                "year" => searchDto.SortDescending
                    ? query.OrderByDescending(c => c.Year)
                    : query.OrderBy(c => c.Year),
                "make" => searchDto.SortDescending
                    ? query.OrderByDescending(c => c.Make)
                    : query.OrderBy(c => c.Make),
                "model" or _ => searchDto.SortDescending
                    ? query.OrderByDescending(c => c.Model)
                    : query.OrderBy(c => c.Model)
            };

            // Apply pagination
            var cars = query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToList();

            var carDtos = _mapper.Map<List<LuxuryCarDto>>(cars);
            var totalPages = (int)Math.Ceiling(totalCount / (double)searchDto.PageSize);

            var result = new LuxuryCarSearchResultDto
            {
                Cars = carDtos,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = totalPages,
                HasNextPage = searchDto.Page < totalPages,
                HasPreviousPage = searchDto.Page > 1
            };

            _logger.LogInformation("Found {CarCount} luxury cars matching criteria", carDtos.Count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching luxury cars");
            return StatusCode(500, "An error occurred while searching luxury cars");
        }
    }

    /// <summary>
    /// Get luxury car by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LuxuryCarDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LuxuryCarDto>> GetLuxuryCar(Guid id)
    {
        try
        {
            var car = await _unitOfWork.LuxuryCars.FirstOrDefaultAsync(c => c.Id == id);
            
            if (car == null)
            {
                return NotFound($"Luxury car with ID {id} not found");
            }

            var carDto = _mapper.Map<LuxuryCarDto>(car);
            return Ok(carDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving luxury car {CarId}", id);
            return StatusCode(500, "An error occurred while retrieving the luxury car");
        }
    }

    /// <summary>
    /// Create a new luxury car (Company only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Company")]
    [ProducesResponseType(typeof(LuxuryCarDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LuxuryCarDto>> CreateLuxuryCar([FromBody] CreateLuxuryCarDto createCarDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            
            if (user?.CompanyId == null)
            {
                return Forbid("User must be associated with a company to create luxury cars");
            }

            var car = _mapper.Map<LuxuryCar>(createCarDto);
            car.CompanyId = user.CompanyId.Value;

            await _unitOfWork.LuxuryCars.AddAsync(car);
            await _unitOfWork.SaveChangesAsync();

            var createdCar = await _unitOfWork.LuxuryCars.FirstOrDefaultAsync(c => c.Id == car.Id);
            var carDto = _mapper.Map<LuxuryCarDto>(createdCar);

            _logger.LogInformation("Luxury car {Make} {Model} created successfully by company {CompanyId}", 
                car.Make, car.Model, user.CompanyId);

            return CreatedAtAction(nameof(GetLuxuryCar), new { id = car.Id }, carDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating luxury car");
            return StatusCode(500, "An error occurred while creating the luxury car");
        }
    }

    /// <summary>
    /// Update luxury car (Company only)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Company")]
    [ProducesResponseType(typeof(LuxuryCarDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LuxuryCarDto>> UpdateLuxuryCar(Guid id, [FromBody] UpdateLuxuryCarDto updateCarDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            
            var car = await _unitOfWork.LuxuryCars.FirstOrDefaultAsync(c => c.Id == id);
            
            if (car == null)
            {
                return NotFound($"Luxury car with ID {id} not found");
            }

            if (car.CompanyId != user?.CompanyId)
            {
                return Forbid("You can only update luxury cars belonging to your company");
            }

            // Update properties
            if (!string.IsNullOrEmpty(updateCarDto.Make))
                car.Make = updateCarDto.Make;
            if (!string.IsNullOrEmpty(updateCarDto.Model))
                car.Model = updateCarDto.Model;
            if (updateCarDto.Year.HasValue)
                car.Year = updateCarDto.Year.Value;
            if (updateCarDto.Type.HasValue)
                car.Type = updateCarDto.Type.Value;
            if (updateCarDto.LuxuryLevel.HasValue)
                car.LuxuryLevel = updateCarDto.LuxuryLevel.Value;
            if (!string.IsNullOrEmpty(updateCarDto.Color))
                car.Color = updateCarDto.Color;
            if (updateCarDto.Seats.HasValue)
                car.Seats = updateCarDto.Seats.Value;
            if (updateCarDto.Description != null)
                car.Description = updateCarDto.Description;
            if (updateCarDto.ImageUrl != null)
                car.ImageUrl = updateCarDto.ImageUrl;
            if (updateCarDto.Features != null)
                car.Features = updateCarDto.Features;
            if (updateCarDto.ImageGallery != null)
                car.ImageGallery = updateCarDto.ImageGallery;
            if (updateCarDto.Location != null)
                car.Location = updateCarDto.Location;
            if (updateCarDto.City != null)
                car.City = updateCarDto.City;
            if (updateCarDto.Country != null)
                car.Country = updateCarDto.Country;
            if (updateCarDto.Latitude.HasValue)
                car.Latitude = updateCarDto.Latitude.Value;
            if (updateCarDto.Longitude.HasValue)
                car.Longitude = updateCarDto.Longitude.Value;
            if (updateCarDto.IsAvailable.HasValue)
                car.IsAvailable = updateCarDto.IsAvailable.Value;
            if (updateCarDto.HourlyRate.HasValue)
                car.HourlyRate = updateCarDto.HourlyRate.Value;
            if (updateCarDto.DailyRate.HasValue)
                car.DailyRate = updateCarDto.DailyRate.Value;
            if (updateCarDto.WithDriver.HasValue)
                car.WithDriver = updateCarDto.WithDriver.Value;
            if (updateCarDto.LicenseRequired != null)
                car.LicenseRequired = updateCarDto.LicenseRequired;
            if (updateCarDto.MinRentalHours.HasValue)
                car.MinRentalHours = updateCarDto.MinRentalHours.Value;

            car.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.LuxuryCars.Update(car);
            await _unitOfWork.SaveChangesAsync();

            var updatedCar = await _unitOfWork.LuxuryCars.FirstOrDefaultAsync(c => c.Id == id);
            var carDto = _mapper.Map<LuxuryCarDto>(updatedCar);

            _logger.LogInformation("Luxury car {CarId} updated successfully", id);
            return Ok(carDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating luxury car {CarId}", id);
            return StatusCode(500, "An error occurred while updating the luxury car");
        }
    }

    /// <summary>
    /// Delete luxury car (Company only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Company")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteLuxuryCar(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            
            var car = await _unitOfWork.LuxuryCars.FirstOrDefaultAsync(c => c.Id == id);
            
            if (car == null)
            {
                return NotFound($"Luxury car with ID {id} not found");
            }

            if (car.CompanyId != user?.CompanyId)
            {
                return Forbid("You can only delete luxury cars belonging to your company");
            }

            // Check if car has bookings
            var hasBookings = await _unitOfWork.CarBookings.AnyAsync(b => b.CarId == id);
            if (hasBookings)
            {
                return BadRequest("Cannot delete luxury car with existing bookings");
            }

            await _unitOfWork.LuxuryCars.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Luxury car {CarId} deleted successfully", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting luxury car {CarId}", id);
            return StatusCode(500, "An error occurred while deleting the luxury car");
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
        return Guid.Parse(userIdClaim?.Value ?? throw new UnauthorizedAccessException("User ID not found in token"));
    }
}