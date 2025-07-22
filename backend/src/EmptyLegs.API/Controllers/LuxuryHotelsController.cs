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
public class LuxuryHotelsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<LuxuryHotelsController> _logger;

    public LuxuryHotelsController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<LuxuryHotelsController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Search for luxury hotels
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(LuxuryHotelSearchResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<LuxuryHotelSearchResultDto>> SearchLuxuryHotels([FromQuery] LuxuryHotelSearchDto searchDto)
    {
        try
        {
            _logger.LogInformation("Searching luxury hotels with criteria: {@SearchCriteria}", searchDto);

            var query = await _unitOfWork.LuxuryHotels.GetAllAsync();
            
            // Apply filters
            if (!string.IsNullOrEmpty(searchDto.Location))
            {
                query = query.Where(h => h.City.Contains(searchDto.Location, StringComparison.OrdinalIgnoreCase) ||
                                        h.Country.Contains(searchDto.Location, StringComparison.OrdinalIgnoreCase) ||
                                        h.Address.Contains(searchDto.Location, StringComparison.OrdinalIgnoreCase));
            }

            if (searchDto.MinStars.HasValue)
            {
                query = query.Where(h => h.Stars >= searchDto.MinStars.Value);
            }

            if (searchDto.LuxuryLevel.HasValue)
            {
                query = query.Where(h => h.LuxuryLevel == searchDto.LuxuryLevel.Value);
            }

            // Filter by amenities
            if (searchDto.Amenities?.Any() == true)
            {
                foreach (var amenity in searchDto.Amenities)
                {
                    query = query.Where(h => h.Amenities.Contains(amenity));
                }
            }

            // Only show active hotels
            query = query.Where(h => h.IsActive);

            // Count total results
            var totalCount = query.Count();

            // Apply sorting
            query = searchDto.SortBy?.ToLower() switch
            {
                "averagerating" or "rating" => searchDto.SortDescending 
                    ? query.OrderByDescending(h => h.AverageRating)
                    : query.OrderBy(h => h.AverageRating),
                "stars" => searchDto.SortDescending
                    ? query.OrderByDescending(h => h.Stars)
                    : query.OrderBy(h => h.Stars),
                "name" or _ => searchDto.SortDescending
                    ? query.OrderByDescending(h => h.Name)
                    : query.OrderBy(h => h.Name)
            };

            // Apply pagination
            var hotels = query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToList();

            var hotelDtos = _mapper.Map<List<LuxuryHotelDto>>(hotels);
            var totalPages = (int)Math.Ceiling(totalCount / (double)searchDto.PageSize);

            var result = new LuxuryHotelSearchResultDto
            {
                Hotels = hotelDtos,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = totalPages,
                HasNextPage = searchDto.Page < totalPages,
                HasPreviousPage = searchDto.Page > 1
            };

            _logger.LogInformation("Found {HotelCount} luxury hotels matching criteria", hotelDtos.Count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching luxury hotels");
            return StatusCode(500, "An error occurred while searching luxury hotels");
        }
    }

    /// <summary>
    /// Get luxury hotel by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LuxuryHotelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LuxuryHotelDto>> GetLuxuryHotel(Guid id)
    {
        try
        {
            var hotel = await _unitOfWork.LuxuryHotels.FirstOrDefaultAsync(h => h.Id == id);
            
            if (hotel == null)
            {
                return NotFound($"Luxury hotel with ID {id} not found");
            }

            var hotelDto = _mapper.Map<LuxuryHotelDto>(hotel);
            return Ok(hotelDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving luxury hotel {HotelId}", id);
            return StatusCode(500, "An error occurred while retrieving the luxury hotel");
        }
    }

    /// <summary>
    /// Create a new luxury hotel (Company only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Company")]
    [ProducesResponseType(typeof(LuxuryHotelDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LuxuryHotelDto>> CreateLuxuryHotel([FromBody] CreateLuxuryHotelDto createHotelDto)
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
                return Forbid("User must be associated with a company to create luxury hotels");
            }

            var hotel = _mapper.Map<LuxuryHotel>(createHotelDto);
            hotel.CompanyId = user.CompanyId.Value;

            await _unitOfWork.LuxuryHotels.AddAsync(hotel);
            await _unitOfWork.SaveChangesAsync();

            var createdHotel = await _unitOfWork.LuxuryHotels.FirstOrDefaultAsync(h => h.Id == hotel.Id);
            var hotelDto = _mapper.Map<LuxuryHotelDto>(createdHotel);

            _logger.LogInformation("Luxury hotel {HotelName} created successfully by company {CompanyId}", 
                hotel.Name, user.CompanyId);

            return CreatedAtAction(nameof(GetLuxuryHotel), new { id = hotel.Id }, hotelDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating luxury hotel");
            return StatusCode(500, "An error occurred while creating the luxury hotel");
        }
    }

    /// <summary>
    /// Update luxury hotel (Company only)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Company")]
    [ProducesResponseType(typeof(LuxuryHotelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LuxuryHotelDto>> UpdateLuxuryHotel(Guid id, [FromBody] UpdateLuxuryHotelDto updateHotelDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            
            var hotel = await _unitOfWork.LuxuryHotels.FirstOrDefaultAsync(h => h.Id == id);
            
            if (hotel == null)
            {
                return NotFound($"Luxury hotel with ID {id} not found");
            }

            if (hotel.CompanyId != user?.CompanyId)
            {
                return Forbid("You can only update luxury hotels belonging to your company");
            }

            // Update properties
            if (!string.IsNullOrEmpty(updateHotelDto.Name))
                hotel.Name = updateHotelDto.Name;
            if (!string.IsNullOrEmpty(updateHotelDto.Brand))
                hotel.Brand = updateHotelDto.Brand;
            if (updateHotelDto.Stars.HasValue)
                hotel.Stars = updateHotelDto.Stars.Value;
            if (updateHotelDto.LuxuryLevel.HasValue)
                hotel.LuxuryLevel = updateHotelDto.LuxuryLevel.Value;
            if (updateHotelDto.Description != null)
                hotel.Description = updateHotelDto.Description;
            if (updateHotelDto.Address != null)
                hotel.Address = updateHotelDto.Address;
            if (updateHotelDto.City != null)
                hotel.City = updateHotelDto.City;
            if (updateHotelDto.Country != null)
                hotel.Country = updateHotelDto.Country;
            if (updateHotelDto.Latitude.HasValue)
                hotel.Latitude = updateHotelDto.Latitude.Value;
            if (updateHotelDto.Longitude.HasValue)
                hotel.Longitude = updateHotelDto.Longitude.Value;
            if (updateHotelDto.ImageUrl != null)
                hotel.ImageUrl = updateHotelDto.ImageUrl;
            if (updateHotelDto.Amenities != null)
                hotel.Amenities = updateHotelDto.Amenities;
            if (updateHotelDto.ImageGallery != null)
                hotel.ImageGallery = updateHotelDto.ImageGallery;
            if (updateHotelDto.Website != null)
                hotel.Website = updateHotelDto.Website;
            if (updateHotelDto.ContactEmail != null)
                hotel.ContactEmail = updateHotelDto.ContactEmail;
            if (updateHotelDto.ContactPhone != null)
                hotel.ContactPhone = updateHotelDto.ContactPhone;
            if (updateHotelDto.IsActive.HasValue)
                hotel.IsActive = updateHotelDto.IsActive.Value;

            hotel.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.LuxuryHotels.Update(hotel);
            await _unitOfWork.SaveChangesAsync();

            var updatedHotel = await _unitOfWork.LuxuryHotels.FirstOrDefaultAsync(h => h.Id == id);
            var hotelDto = _mapper.Map<LuxuryHotelDto>(updatedHotel);

            _logger.LogInformation("Luxury hotel {HotelId} updated successfully", id);
            return Ok(hotelDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating luxury hotel {HotelId}", id);
            return StatusCode(500, "An error occurred while updating the luxury hotel");
        }
    }

    /// <summary>
    /// Delete luxury hotel (Company only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Company")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteLuxuryHotel(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            
            var hotel = await _unitOfWork.LuxuryHotels.FirstOrDefaultAsync(h => h.Id == id);
            
            if (hotel == null)
            {
                return NotFound($"Luxury hotel with ID {id} not found");
            }

            if (hotel.CompanyId != user?.CompanyId)
            {
                return Forbid("You can only delete luxury hotels belonging to your company");
            }

            // Check if hotel has bookings
            var hasBookings = await _unitOfWork.HotelBookings.AnyAsync(b => b.HotelId == id);
            if (hasBookings)
            {
                return BadRequest("Cannot delete luxury hotel with existing bookings");
            }

            await _unitOfWork.LuxuryHotels.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Luxury hotel {HotelId} deleted successfully", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting luxury hotel {HotelId}", id);
            return StatusCode(500, "An error occurred while deleting the luxury hotel");
        }
    }

    /// <summary>
    /// Get hotel rooms
    /// </summary>
    [HttpGet("{id:guid}/rooms")]
    [ProducesResponseType(typeof(IEnumerable<HotelRoomDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<HotelRoomDto>>> GetHotelRooms(Guid id)
    {
        try
        {
            var hotel = await _unitOfWork.LuxuryHotels.FirstOrDefaultAsync(h => h.Id == id);
            
            if (hotel == null)
            {
                return NotFound($"Luxury hotel with ID {id} not found");
            }

            var rooms = await _unitOfWork.HotelRooms.FindAsync(r => r.HotelId == id && r.IsAvailable);
            var roomDtos = _mapper.Map<IEnumerable<HotelRoomDto>>(rooms);

            return Ok(roomDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving rooms for hotel {HotelId}", id);
            return StatusCode(500, "An error occurred while retrieving hotel rooms");
        }
    }

    /// <summary>
    /// Add room to hotel (Company only)
    /// </summary>
    [HttpPost("{id:guid}/rooms")]
    [Authorize(Roles = "Company")]
    [ProducesResponseType(typeof(HotelRoomDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HotelRoomDto>> AddHotelRoom(Guid id, [FromBody] CreateHotelRoomDto createRoomDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            
            var hotel = await _unitOfWork.LuxuryHotels.FirstOrDefaultAsync(h => h.Id == id);
            
            if (hotel == null)
            {
                return NotFound($"Luxury hotel with ID {id} not found");
            }

            if (hotel.CompanyId != user?.CompanyId)
            {
                return Forbid("You can only add rooms to hotels belonging to your company");
            }

            var room = _mapper.Map<HotelRoom>(createRoomDto);
            room.HotelId = id;

            await _unitOfWork.HotelRooms.AddAsync(room);
            await _unitOfWork.SaveChangesAsync();

            var roomDto = _mapper.Map<HotelRoomDto>(room);

            _logger.LogInformation("Room {RoomNumber} added to hotel {HotelId}", room.RoomNumber, id);
            return CreatedAtAction(nameof(GetHotelRooms), new { id }, roomDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding room to hotel {HotelId}", id);
            return StatusCode(500, "An error occurred while adding the room");
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
        return Guid.Parse(userIdClaim?.Value ?? throw new UnauthorizedAccessException("User ID not found in token"));
    }
}