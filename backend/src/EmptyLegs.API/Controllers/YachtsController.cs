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
public class YachtsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<YachtsController> _logger;

    public YachtsController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<YachtsController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Search for available yachts
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(YachtSearchResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<YachtSearchResultDto>> SearchYachts([FromQuery] YachtSearchDto searchDto)
    {
        try
        {
            _logger.LogInformation("Searching yachts with criteria: {@SearchCriteria}", searchDto);

            var query = await _unitOfWork.Yachts.GetAllAsync();
            
            // Apply filters
            if (!string.IsNullOrEmpty(searchDto.Location))
            {
                query = query.Where(y => y.HomePort.Contains(searchDto.Location, StringComparison.OrdinalIgnoreCase));
            }

            if (searchDto.MaxGuests.HasValue)
            {
                query = query.Where(y => y.MaxGuests >= searchDto.MaxGuests.Value);
            }

            if (searchDto.Type.HasValue)
            {
                query = query.Where(y => y.Type == searchDto.Type.Value);
            }

            if (searchDto.LuxuryLevel.HasValue)
            {
                query = query.Where(y => y.LuxuryLevel == searchDto.LuxuryLevel.Value);
            }

            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(y => y.DailyRate <= searchDto.MaxPrice.Value);
            }

            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(y => y.DailyRate >= searchDto.MinPrice.Value);
            }

            if (searchDto.MinLength.HasValue)
            {
                query = query.Where(y => y.Length >= searchDto.MinLength.Value);
            }

            if (searchDto.MaxLength.HasValue)
            {
                query = query.Where(y => y.Length <= searchDto.MaxLength.Value);
            }

            // Filter by amenities
            if (searchDto.Amenities?.Any() == true)
            {
                foreach (var amenity in searchDto.Amenities)
                {
                    query = query.Where(y => y.Amenities.Contains(amenity));
                }
            }

            // Only show available yachts
            query = query.Where(y => y.IsAvailable);

            // Count total results
            var totalCount = query.Count();

            // Apply sorting
            query = searchDto.SortBy?.ToLower() switch
            {
                "dailyrate" or "price" => searchDto.SortDescending 
                    ? query.OrderByDescending(y => y.DailyRate)
                    : query.OrderBy(y => y.DailyRate),
                "length" => searchDto.SortDescending
                    ? query.OrderByDescending(y => y.Length)
                    : query.OrderBy(y => y.Length),
                "maxguests" => searchDto.SortDescending
                    ? query.OrderByDescending(y => y.MaxGuests)
                    : query.OrderBy(y => y.MaxGuests),
                "name" or _ => searchDto.SortDescending
                    ? query.OrderByDescending(y => y.Name)
                    : query.OrderBy(y => y.Name)
            };

            // Apply pagination
            var yachts = query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToList();

            var yachtDtos = _mapper.Map<List<YachtDto>>(yachts);
            var totalPages = (int)Math.Ceiling(totalCount / (double)searchDto.PageSize);

            var result = new YachtSearchResultDto
            {
                Yachts = yachtDtos,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = totalPages,
                HasNextPage = searchDto.Page < totalPages,
                HasPreviousPage = searchDto.Page > 1
            };

            _logger.LogInformation("Found {YachtCount} yachts matching criteria", yachtDtos.Count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching yachts");
            return StatusCode(500, "An error occurred while searching yachts");
        }
    }

    /// <summary>
    /// Get yacht by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(YachtDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<YachtDto>> GetYacht(Guid id)
    {
        try
        {
            var yacht = await _unitOfWork.Yachts.FirstOrDefaultAsync(y => y.Id == id);
            
            if (yacht == null)
            {
                return NotFound($"Yacht with ID {id} not found");
            }

            var yachtDto = _mapper.Map<YachtDto>(yacht);
            return Ok(yachtDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving yacht {YachtId}", id);
            return StatusCode(500, "An error occurred while retrieving the yacht");
        }
    }

    /// <summary>
    /// Create a new yacht (Company only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Company")]
    [ProducesResponseType(typeof(YachtDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<YachtDto>> CreateYacht([FromBody] CreateYachtDto createYachtDto)
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
                return Forbid("User must be associated with a company to create yachts");
            }

            var yacht = _mapper.Map<Yacht>(createYachtDto);
            yacht.CompanyId = user.CompanyId.Value;

            await _unitOfWork.Yachts.AddAsync(yacht);
            await _unitOfWork.SaveChangesAsync();

            var createdYacht = await _unitOfWork.Yachts.FirstOrDefaultAsync(y => y.Id == yacht.Id);
            var yachtDto = _mapper.Map<YachtDto>(createdYacht);

            _logger.LogInformation("Yacht {YachtName} created successfully by company {CompanyId}", 
                yacht.Name, user.CompanyId);

            return CreatedAtAction(nameof(GetYacht), new { id = yacht.Id }, yachtDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating yacht");
            return StatusCode(500, "An error occurred while creating the yacht");
        }
    }

    /// <summary>
    /// Update yacht (Company only)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Company")]
    [ProducesResponseType(typeof(YachtDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<YachtDto>> UpdateYacht(Guid id, [FromBody] UpdateYachtDto updateYachtDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            
            var yacht = await _unitOfWork.Yachts.FirstOrDefaultAsync(y => y.Id == id);
            
            if (yacht == null)
            {
                return NotFound($"Yacht with ID {id} not found");
            }

            if (yacht.CompanyId != user?.CompanyId)
            {
                return Forbid("You can only update yachts belonging to your company");
            }

            // Update properties
            if (!string.IsNullOrEmpty(updateYachtDto.Name))
                yacht.Name = updateYachtDto.Name;
            if (!string.IsNullOrEmpty(updateYachtDto.Model))
                yacht.Model = updateYachtDto.Model;
            if (!string.IsNullOrEmpty(updateYachtDto.Manufacturer))
                yacht.Manufacturer = updateYachtDto.Manufacturer;
            if (updateYachtDto.Year.HasValue)
                yacht.Year = updateYachtDto.Year.Value;
            if (updateYachtDto.Length.HasValue)
                yacht.Length = updateYachtDto.Length.Value;
            if (updateYachtDto.MaxGuests.HasValue)
                yacht.MaxGuests = updateYachtDto.MaxGuests.Value;
            if (updateYachtDto.Cabins.HasValue)
                yacht.Cabins = updateYachtDto.Cabins.Value;
            if (updateYachtDto.Crew.HasValue)
                yacht.Crew = updateYachtDto.Crew.Value;
            if (updateYachtDto.Type.HasValue)
                yacht.Type = updateYachtDto.Type.Value;
            if (updateYachtDto.LuxuryLevel.HasValue)
                yacht.LuxuryLevel = updateYachtDto.LuxuryLevel.Value;
            if (updateYachtDto.Description != null)
                yacht.Description = updateYachtDto.Description;
            if (updateYachtDto.ImageUrl != null)
                yacht.ImageUrl = updateYachtDto.ImageUrl;
            if (updateYachtDto.Amenities != null)
                yacht.Amenities = updateYachtDto.Amenities;
            if (updateYachtDto.ImageGallery != null)
                yacht.ImageGallery = updateYachtDto.ImageGallery;
            if (updateYachtDto.HomePort != null)
                yacht.HomePort = updateYachtDto.HomePort;
            if (updateYachtDto.Latitude.HasValue)
                yacht.Latitude = updateYachtDto.Latitude.Value;
            if (updateYachtDto.Longitude.HasValue)
                yacht.Longitude = updateYachtDto.Longitude.Value;
            if (updateYachtDto.IsAvailable.HasValue)
                yacht.IsAvailable = updateYachtDto.IsAvailable.Value;
            if (updateYachtDto.DailyRate.HasValue)
                yacht.DailyRate = updateYachtDto.DailyRate.Value;
            if (updateYachtDto.WeeklyRate.HasValue)
                yacht.WeeklyRate = updateYachtDto.WeeklyRate.Value;
            if (updateYachtDto.SpecialFeatures != null)
                yacht.SpecialFeatures = updateYachtDto.SpecialFeatures;

            yacht.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Yachts.Update(yacht);
            await _unitOfWork.SaveChangesAsync();

            var updatedYacht = await _unitOfWork.Yachts.FirstOrDefaultAsync(y => y.Id == id);
            var yachtDto = _mapper.Map<YachtDto>(updatedYacht);

            _logger.LogInformation("Yacht {YachtId} updated successfully", id);
            return Ok(yachtDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating yacht {YachtId}", id);
            return StatusCode(500, "An error occurred while updating the yacht");
        }
    }

    /// <summary>
    /// Delete yacht (Company only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Company")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteYacht(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            
            var yacht = await _unitOfWork.Yachts.FirstOrDefaultAsync(y => y.Id == id);
            
            if (yacht == null)
            {
                return NotFound($"Yacht with ID {id} not found");
            }

            if (yacht.CompanyId != user?.CompanyId)
            {
                return Forbid("You can only delete yachts belonging to your company");
            }

            // Check if yacht has bookings
            var hasBookings = await _unitOfWork.YachtBookings.AnyAsync(b => b.YachtId == id);
            if (hasBookings)
            {
                return BadRequest("Cannot delete yacht with existing bookings");
            }

            await _unitOfWork.Yachts.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Yacht {YachtId} deleted successfully", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting yacht {YachtId}", id);
            return StatusCode(500, "An error occurred while deleting the yacht");
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
        return Guid.Parse(userIdClaim?.Value ?? throw new UnauthorizedAccessException("User ID not found in token"));
    }
}