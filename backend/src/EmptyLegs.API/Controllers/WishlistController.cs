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
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<WishlistController> _logger;

    public WishlistController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<WishlistController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get user's wishlists
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WishlistDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WishlistDto>>> GetWishlists()
    {
        try
        {
            var userId = GetCurrentUserId();
            var wishlists = await _unitOfWork.Wishlists.FindAsync(w => w.UserId == userId);
            
            var wishlistDtos = new List<WishlistDto>();
            
            foreach (var wishlist in wishlists)
            {
                var wishlistDto = _mapper.Map<WishlistDto>(wishlist);
                
                // Populate vehicle details for each item
                foreach (var item in wishlistDto.Items)
                {
                    item.VehicleDetails = await GetVehicleDetails(item.VehicleType, item.VehicleId);
                }
                
                wishlistDtos.Add(wishlistDto);
            }

            return Ok(wishlistDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving wishlists for user");
            return StatusCode(500, "An error occurred while retrieving wishlists");
        }
    }

    /// <summary>
    /// Get wishlist by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WishlistDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<WishlistDto>> GetWishlist(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var wishlist = await _unitOfWork.Wishlists.FirstOrDefaultAsync(w => w.Id == id);
            
            if (wishlist == null)
            {
                return NotFound("Wishlist not found");
            }

            // Check if user owns this wishlist or if it's public
            if (wishlist.UserId != userId && !wishlist.IsPublic)
            {
                return Forbid("You can only access your own wishlists or public ones");
            }

            var wishlistDto = _mapper.Map<WishlistDto>(wishlist);
            
            // Populate vehicle details for each item
            foreach (var item in wishlistDto.Items)
            {
                item.VehicleDetails = await GetVehicleDetails(item.VehicleType, item.VehicleId);
            }

            return Ok(wishlistDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving wishlist {WishlistId}", id);
            return StatusCode(500, "An error occurred while retrieving the wishlist");
        }
    }

    /// <summary>
    /// Create a new wishlist
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WishlistDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WishlistDto>> CreateWishlist([FromBody] CreateWishlistDto createWishlistDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            
            var wishlist = _mapper.Map<Wishlist>(createWishlistDto);
            wishlist.UserId = userId;

            await _unitOfWork.Wishlists.AddAsync(wishlist);
            await _unitOfWork.SaveChangesAsync();

            var wishlistDto = _mapper.Map<WishlistDto>(wishlist);

            _logger.LogInformation("Wishlist {WishlistName} created for user {UserId}", wishlist.Name, userId);
            return CreatedAtAction(nameof(GetWishlist), new { id = wishlist.Id }, wishlistDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating wishlist");
            return StatusCode(500, "An error occurred while creating the wishlist");
        }
    }

    /// <summary>
    /// Update wishlist
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WishlistDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<WishlistDto>> UpdateWishlist(Guid id, [FromBody] UpdateWishlistDto updateWishlistDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var wishlist = await _unitOfWork.Wishlists.FirstOrDefaultAsync(w => w.Id == id);
            
            if (wishlist == null)
            {
                return NotFound("Wishlist not found");
            }

            if (wishlist.UserId != userId)
            {
                return Forbid("You can only update your own wishlists");
            }

            // Update properties
            if (!string.IsNullOrEmpty(updateWishlistDto.Name))
                wishlist.Name = updateWishlistDto.Name;
            if (updateWishlistDto.Description != null)
                wishlist.Description = updateWishlistDto.Description;
            if (updateWishlistDto.IsPublic.HasValue)
                wishlist.IsPublic = updateWishlistDto.IsPublic.Value;

            wishlist.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Wishlists.Update(wishlist);
            await _unitOfWork.SaveChangesAsync();

            var wishlistDto = _mapper.Map<WishlistDto>(wishlist);

            _logger.LogInformation("Wishlist {WishlistId} updated", id);
            return Ok(wishlistDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating wishlist {WishlistId}", id);
            return StatusCode(500, "An error occurred while updating the wishlist");
        }
    }

    /// <summary>
    /// Delete wishlist
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteWishlist(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var wishlist = await _unitOfWork.Wishlists.FirstOrDefaultAsync(w => w.Id == id);
            
            if (wishlist == null)
            {
                return NotFound("Wishlist not found");
            }

            if (wishlist.UserId != userId)
            {
                return Forbid("You can only delete your own wishlists");
            }

            await _unitOfWork.Wishlists.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Wishlist {WishlistId} deleted", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting wishlist {WishlistId}", id);
            return StatusCode(500, "An error occurred while deleting the wishlist");
        }
    }

    /// <summary>
    /// Add item to wishlist
    /// </summary>
    [HttpPost("items")]
    [ProducesResponseType(typeof(WishlistItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishlistItemDto>> AddToWishlist([FromBody] AddToWishlistDto addToWishlistDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var wishlist = await _unitOfWork.Wishlists.FirstOrDefaultAsync(w => w.Id == addToWishlistDto.WishlistId);
            
            if (wishlist == null)
            {
                return NotFound("Wishlist not found");
            }

            if (wishlist.UserId != userId)
            {
                return Forbid("You can only add items to your own wishlists");
            }

            // Check if vehicle exists
            var vehicleExists = await ValidateVehicleExists(addToWishlistDto.VehicleType, addToWishlistDto.VehicleId);
            if (!vehicleExists)
            {
                return BadRequest("Vehicle not found");
            }

            // Check if item already exists in wishlist
            var existingItem = await _unitOfWork.WishlistItems.FirstOrDefaultAsync(i => 
                i.WishlistId == addToWishlistDto.WishlistId && 
                i.VehicleType == addToWishlistDto.VehicleType && 
                i.VehicleId == addToWishlistDto.VehicleId);

            if (existingItem != null)
            {
                return BadRequest("Item already exists in wishlist");
            }

            var wishlistItem = new WishlistItem
            {
                WishlistId = addToWishlistDto.WishlistId,
                VehicleType = addToWishlistDto.VehicleType,
                VehicleId = addToWishlistDto.VehicleId,
                Notes = addToWishlistDto.Notes
            };

            await _unitOfWork.WishlistItems.AddAsync(wishlistItem);
            await _unitOfWork.SaveChangesAsync();

            var itemDto = _mapper.Map<WishlistItemDto>(wishlistItem);
            itemDto.VehicleDetails = await GetVehicleDetails(wishlistItem.VehicleType, wishlistItem.VehicleId);

            _logger.LogInformation("Item added to wishlist {WishlistId}", addToWishlistDto.WishlistId);
            return CreatedAtAction(nameof(GetWishlist), new { id = addToWishlistDto.WishlistId }, itemDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to wishlist");
            return StatusCode(500, "An error occurred while adding item to wishlist");
        }
    }

    /// <summary>
    /// Remove item from wishlist
    /// </summary>
    [HttpDelete("items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> RemoveFromWishlist([FromBody] RemoveFromWishlistDto removeFromWishlistDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var wishlist = await _unitOfWork.Wishlists.FirstOrDefaultAsync(w => w.Id == removeFromWishlistDto.WishlistId);
            
            if (wishlist == null)
            {
                return NotFound("Wishlist not found");
            }

            if (wishlist.UserId != userId)
            {
                return Forbid("You can only remove items from your own wishlists");
            }

            var item = await _unitOfWork.WishlistItems.FirstOrDefaultAsync(i => i.Id == removeFromWishlistDto.ItemId);
            
            if (item == null)
            {
                return NotFound("Wishlist item not found");
            }

            await _unitOfWork.WishlistItems.SoftDeleteAsync(removeFromWishlistDto.ItemId);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Item {ItemId} removed from wishlist {WishlistId}", 
                removeFromWishlistDto.ItemId, removeFromWishlistDto.WishlistId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item from wishlist");
            return StatusCode(500, "An error occurred while removing item from wishlist");
        }
    }

    private async Task<object?> GetVehicleDetails(VehicleType vehicleType, Guid vehicleId)
    {
        return vehicleType switch
        {
            VehicleType.Aircraft => await _unitOfWork.Aircraft.FirstOrDefaultAsync(a => a.Id == vehicleId),
            VehicleType.Yacht => await _unitOfWork.Yachts.FirstOrDefaultAsync(y => y.Id == vehicleId),
            VehicleType.LuxuryCar => await _unitOfWork.LuxuryCars.FirstOrDefaultAsync(c => c.Id == vehicleId),
            VehicleType.Hotel => await _unitOfWork.LuxuryHotels.FirstOrDefaultAsync(h => h.Id == vehicleId),
            _ => null
        };
    }

    private async Task<bool> ValidateVehicleExists(VehicleType vehicleType, Guid vehicleId)
    {
        return vehicleType switch
        {
            VehicleType.Aircraft => await _unitOfWork.Aircraft.AnyAsync(a => a.Id == vehicleId),
            VehicleType.Yacht => await _unitOfWork.Yachts.AnyAsync(y => y.Id == vehicleId),
            VehicleType.LuxuryCar => await _unitOfWork.LuxuryCars.AnyAsync(c => c.Id == vehicleId),
            VehicleType.Hotel => await _unitOfWork.LuxuryHotels.AnyAsync(h => h.Id == vehicleId),
            _ => false
        };
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
        return Guid.Parse(userIdClaim?.Value ?? throw new UnauthorizedAccessException("User ID not found in token"));
    }
}