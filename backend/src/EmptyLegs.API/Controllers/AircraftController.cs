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
public class AircraftController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AircraftController> _logger;

    public AircraftController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AircraftController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all aircraft
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="type">Filter by aircraft type</param>
    /// <param name="companyId">Filter by company ID</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="manufacturer">Filter by manufacturer</param>
    /// <returns>Paginated list of aircraft</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AircraftDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AircraftDto>>> GetAircraft(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] AircraftType? type = null,
        [FromQuery] Guid? companyId = null,
        [FromQuery] bool? isActive = true,
        [FromQuery] string? manufacturer = null)
    {
        try
        {
            IEnumerable<Aircraft> aircraft;

            if (type.HasValue || companyId.HasValue || isActive.HasValue || !string.IsNullOrEmpty(manufacturer))
            {
                aircraft = await _unitOfWork.Aircraft.FindAsync(a =>
                    (!type.HasValue || a.Type == type.Value) &&
                    (!companyId.HasValue || a.CompanyId == companyId.Value) &&
                    (!isActive.HasValue || a.IsActive == isActive.Value) &&
                    (string.IsNullOrEmpty(manufacturer) || a.Manufacturer.Contains(manufacturer)));
            }
            else
            {
                aircraft = await _unitOfWork.Aircraft.GetPagedAsync(page, pageSize);
            }

            var aircraftDtos = _mapper.Map<IEnumerable<AircraftDto>>(aircraft);
            
            _logger.LogInformation("Retrieved {Count} aircraft for page {Page}", aircraftDtos.Count(), page);
            return Ok(aircraftDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving aircraft");
            return StatusCode(500, new { message = "An error occurred while retrieving aircraft" });
        }
    }

    /// <summary>
    /// Get aircraft by ID
    /// </summary>
    /// <param name="id">Aircraft ID</param>
    /// <returns>Aircraft details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AircraftDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AircraftDto>> GetAircraft(Guid id)
    {
        try
        {
            var aircraft = await _unitOfWork.Aircraft.GetByIdAsync(id);
            if (aircraft == null)
            {
                return NotFound(new { message = "Aircraft not found" });
            }

            var aircraftDto = _mapper.Map<AircraftDto>(aircraft);
            return Ok(aircraftDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving aircraft {AircraftId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the aircraft" });
        }
    }

    /// <summary>
    /// Search aircraft by text
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="limit">Maximum number of results</param>
    /// <returns>List of matching aircraft</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<AircraftDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AircraftDto>>> SearchAircraft(
        [FromQuery] string query,
        [FromQuery] int limit = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { message = "Search query is required" });
            }

            var aircraft = await _unitOfWork.Aircraft.FindAsync(a =>
                a.IsActive &&
                (a.Model.Contains(query) ||
                 a.Manufacturer.Contains(query) ||
                 a.Registration.Contains(query)));

            var limitedAircraft = aircraft.Take(limit);
            var aircraftDtos = _mapper.Map<IEnumerable<AircraftDto>>(limitedAircraft);
            
            _logger.LogInformation("Found {Count} aircraft matching query '{Query}'", aircraftDtos.Count(), query);
            return Ok(aircraftDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching aircraft with query '{Query}'", query);
            return StatusCode(500, new { message = "An error occurred while searching aircraft" });
        }
    }

    /// <summary>
    /// Create new aircraft (Company/Admin only)
    /// </summary>
    /// <param name="createAircraftDto">Aircraft data</param>
    /// <returns>Created aircraft details</returns>
    [HttpPost]
    [Authorize(Roles = "Company,Admin")]
    [ProducesResponseType(typeof(AircraftDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AircraftDto>> CreateAircraft([FromBody] CreateAircraftDto createAircraftDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if aircraft with same registration already exists
            var existingAircraft = await _unitOfWork.Aircraft
                .FirstOrDefaultAsync(a => a.Registration == createAircraftDto.Registration);
            
            if (existingAircraft != null)
            {
                return Conflict(new { message = "An aircraft with this registration already exists" });
            }

            // Validate company authorization
            var currentUserRole = GetCurrentUserRole();
            var currentUserCompanyId = GetCurrentUserCompanyId();

            if (currentUserRole == "Company" && currentUserCompanyId != createAircraftDto.CompanyId)
            {
                return Forbid("You can only create aircraft for your own company");
            }

            // Verify company exists
            var company = await _unitOfWork.Companies.GetByIdAsync(createAircraftDto.CompanyId);
            if (company == null)
            {
                return BadRequest(new { message = "Company not found" });
            }

            var aircraft = _mapper.Map<Aircraft>(createAircraftDto);
            aircraft.IsActive = true;
            aircraft.CreatedAt = DateTime.UtcNow;
            aircraft.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Aircraft.AddAsync(aircraft);
            await _unitOfWork.SaveChangesAsync();

            var aircraftDto = _mapper.Map<AircraftDto>(aircraft);
            
            _logger.LogInformation("Aircraft {AircraftId} ({Registration}) created successfully", aircraft.Id, aircraft.Registration);
            return CreatedAtAction(nameof(GetAircraft), new { id = aircraft.Id }, aircraftDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating aircraft");
            return StatusCode(500, new { message = "An error occurred while creating the aircraft" });
        }
    }

    /// <summary>
    /// Update aircraft (Company/Admin only)
    /// </summary>
    /// <param name="id">Aircraft ID</param>
    /// <param name="updateAircraftDto">Updated aircraft data</param>
    /// <returns>Updated aircraft details</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Company,Admin")]
    [ProducesResponseType(typeof(AircraftDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AircraftDto>> UpdateAircraft(Guid id, [FromBody] UpdateAircraftDto updateAircraftDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var aircraft = await _unitOfWork.Aircraft.GetByIdAsync(id);
            if (aircraft == null)
            {
                return NotFound(new { message = "Aircraft not found" });
            }

            // Check authorization - only admins or aircraft's company can update
            var currentUserRole = GetCurrentUserRole();
            var currentUserCompanyId = GetCurrentUserCompanyId();

            if (currentUserRole != "Admin" && currentUserCompanyId != aircraft.CompanyId)
            {
                return Forbid("You can only update your own company's aircraft");
            }

            // Update aircraft properties
            if (!string.IsNullOrEmpty(updateAircraftDto.Model))
                aircraft.Model = updateAircraftDto.Model;
            
            if (!string.IsNullOrEmpty(updateAircraftDto.Manufacturer))
                aircraft.Manufacturer = updateAircraftDto.Manufacturer;
            
            if (updateAircraftDto.Capacity.HasValue)
                aircraft.Capacity = updateAircraftDto.Capacity.Value;
            
            if (updateAircraftDto.YearManufactured.HasValue)
                aircraft.YearManufactured = updateAircraftDto.YearManufactured.Value;
            
            if (!string.IsNullOrEmpty(updateAircraftDto.Description))
                aircraft.Description = updateAircraftDto.Description;
            
            if (updateAircraftDto.CruiseSpeed.HasValue)
                aircraft.CruiseSpeed = updateAircraftDto.CruiseSpeed;
            
            if (updateAircraftDto.Range.HasValue)
                aircraft.Range = updateAircraftDto.Range;

            if (updateAircraftDto.Amenities != null)
                aircraft.Amenities = updateAircraftDto.Amenities;

            if (updateAircraftDto.PhotoUrls != null)
                aircraft.PhotoUrls = updateAircraftDto.PhotoUrls;

            aircraft.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Aircraft.Update(aircraft);
            await _unitOfWork.SaveChangesAsync();

            var aircraftDto = _mapper.Map<AircraftDto>(aircraft);
            
            _logger.LogInformation("Aircraft {AircraftId} updated successfully", id);
            return Ok(aircraftDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating aircraft {AircraftId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the aircraft" });
        }
    }

    /// <summary>
    /// Deactivate aircraft (Company/Admin only)
    /// </summary>
    /// <param name="id">Aircraft ID</param>
    /// <returns>Success status</returns>
    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Roles = "Company,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeactivateAircraft(Guid id)
    {
        try
        {
            var aircraft = await _unitOfWork.Aircraft.GetByIdAsync(id);
            if (aircraft == null)
            {
                return NotFound(new { message = "Aircraft not found" });
            }

            // Check authorization
            var currentUserRole = GetCurrentUserRole();
            var currentUserCompanyId = GetCurrentUserCompanyId();

            if (currentUserRole != "Admin" && currentUserCompanyId != aircraft.CompanyId)
            {
                return Forbid("You can only deactivate your own company's aircraft");
            }

            aircraft.IsActive = false;
            aircraft.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Aircraft.Update(aircraft);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Aircraft {AircraftId} deactivated", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating aircraft {AircraftId}", id);
            return StatusCode(500, new { message = "An error occurred while deactivating the aircraft" });
        }
    }

    /// <summary>
    /// Activate aircraft (Company/Admin only)
    /// </summary>
    /// <param name="id">Aircraft ID</param>
    /// <returns>Success status</returns>
    [HttpPatch("{id:guid}/activate")]
    [Authorize(Roles = "Company,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ActivateAircraft(Guid id)
    {
        try
        {
            var aircraft = await _unitOfWork.Aircraft.GetByIdAsync(id);
            if (aircraft == null)
            {
                return NotFound(new { message = "Aircraft not found" });
            }

            // Check authorization
            var currentUserRole = GetCurrentUserRole();
            var currentUserCompanyId = GetCurrentUserCompanyId();

            if (currentUserRole != "Admin" && currentUserCompanyId != aircraft.CompanyId)
            {
                return Forbid("You can only activate your own company's aircraft");
            }

            aircraft.IsActive = true;
            aircraft.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Aircraft.Update(aircraft);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Aircraft {AircraftId} activated", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating aircraft {AircraftId}", id);
            return StatusCode(500, new { message = "An error occurred while activating the aircraft" });
        }
    }

    /// <summary>
    /// Delete aircraft (Admin only - soft delete)
    /// </summary>
    /// <param name="id">Aircraft ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteAircraft(Guid id)
    {
        try
        {
            var aircraft = await _unitOfWork.Aircraft.GetByIdAsync(id);
            if (aircraft == null)
            {
                return NotFound(new { message = "Aircraft not found" });
            }

            await _unitOfWork.Aircraft.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Aircraft {AircraftId} deleted", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting aircraft {AircraftId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the aircraft" });
        }
    }

    /// <summary>
    /// Get aircraft's flights
    /// </summary>
    /// <param name="id">Aircraft ID</param>
    /// <param name="status">Filter by flight status</param>
    /// <param name="startDate">Filter flights from this date</param>
    /// <param name="endDate">Filter flights until this date</param>
    /// <returns>List of aircraft's flights</returns>
    [HttpGet("{id:guid}/flights")]
    [ProducesResponseType(typeof(IEnumerable<FlightDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<FlightDto>>> GetAircraftFlights(
        Guid id,
        [FromQuery] FlightStatus? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var aircraft = await _unitOfWork.Aircraft.GetByIdAsync(id);
            if (aircraft == null)
            {
                return NotFound(new { message = "Aircraft not found" });
            }

            var flights = await _unitOfWork.Flights.FindAsync(f =>
                f.AircraftId == id &&
                (!status.HasValue || f.Status == status.Value) &&
                (!startDate.HasValue || f.DepartureTime >= startDate.Value) &&
                (!endDate.HasValue || f.DepartureTime <= endDate.Value));

            var flightDtos = _mapper.Map<IEnumerable<FlightDto>>(flights);
            
            _logger.LogInformation("Retrieved {Count} flights for aircraft {AircraftId}", flightDtos.Count(), id);
            return Ok(flightDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving flights for aircraft {AircraftId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving aircraft flights" });
        }
    }

    /// <summary>
    /// Get aircraft types
    /// </summary>
    /// <returns>List of aircraft types</returns>
    [HttpGet("types")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetAircraftTypes()
    {
        try
        {
            var types = Enum.GetValues<AircraftType>()
                .Select(t => new
                {
                    Value = (int)t,
                    Name = t.ToString(),
                    DisplayName = GetAircraftTypeDisplayName(t)
                });

            return Ok(types);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving aircraft types");
            return StatusCode(500, new { message = "An error occurred while retrieving aircraft types" });
        }
    }

    /// <summary>
    /// Get aircraft manufacturers
    /// </summary>
    /// <returns>List of manufacturers</returns>
    [HttpGet("manufacturers")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetManufacturers()
    {
        try
        {
            var aircraft = await _unitOfWork.Aircraft.FindAsync(a => a.IsActive);
            var manufacturers = aircraft
                .Select(a => a.Manufacturer)
                .Distinct()
                .OrderBy(m => m)
                .ToList();

            _logger.LogInformation("Retrieved {Count} aircraft manufacturers", manufacturers.Count);
            return Ok(manufacturers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving aircraft manufacturers");
            return StatusCode(500, new { message = "An error occurred while retrieving manufacturers" });
        }
    }

    /// <summary>
    /// Get aircraft statistics (Admin only)
    /// </summary>
    /// <returns>Aircraft statistics</returns>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<object>> GetAircraftStatistics()
    {
        try
        {
            var totalAircraft = await _unitOfWork.Aircraft.CountAsync();
            var activeAircraft = await _unitOfWork.Aircraft.CountAsync(a => a.IsActive);

            var statistics = new
            {
                Total = totalAircraft,
                Active = activeAircraft,
                Inactive = totalAircraft - activeAircraft,
                ByType = new
                {
                    LightJet = await _unitOfWork.Aircraft.CountAsync(a => a.Type == AircraftType.LightJet),
                    MidSizeJet = await _unitOfWork.Aircraft.CountAsync(a => a.Type == AircraftType.MidSizeJet),
                    HeavyJet = await _unitOfWork.Aircraft.CountAsync(a => a.Type == AircraftType.HeavyJet),
                    UltraLongRange = await _unitOfWork.Aircraft.CountAsync(a => a.Type == AircraftType.UltraLongRange),
                    Turboprop = await _unitOfWork.Aircraft.CountAsync(a => a.Type == AircraftType.Turboprop),
                    Helicopter = await _unitOfWork.Aircraft.CountAsync(a => a.Type == AircraftType.Helicopter)
                },
                AverageCapacity = await CalculateAverageCapacity(),
                TopManufacturers = await GetTopManufacturers()
            };

            _logger.LogInformation("Aircraft statistics retrieved");
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving aircraft statistics");
            return StatusCode(500, new { message = "An error occurred while retrieving aircraft statistics" });
        }
    }

    private async Task<double> CalculateAverageCapacity()
    {
        var aircraft = await _unitOfWork.Aircraft.FindAsync(a => a.IsActive);
        return aircraft.Any() ? aircraft.Average(a => a.Capacity) : 0;
    }

    private async Task<IEnumerable<object>> GetTopManufacturers()
    {
        var aircraft = await _unitOfWork.Aircraft.FindAsync(a => a.IsActive);
        return aircraft
            .GroupBy(a => a.Manufacturer)
            .Select(g => new { Manufacturer = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5);
    }

    private static string GetAircraftTypeDisplayName(AircraftType type)
    {
        return type switch
        {
            AircraftType.LightJet => "Light Jet",
            AircraftType.MidJet or AircraftType.MidSizeJet => "Mid-Size Jet",
            AircraftType.HeavyJet or AircraftType.UltraLongRange => "Ultra Long Range",
            AircraftType.Turboprop => "Turboprop",
            AircraftType.VeryLightJet => "Very Light Jet",
            AircraftType.Helicopter => "Helicopter",
            _ => type.ToString()
        };
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    private Guid? GetCurrentUserCompanyId()
    {
        var companyIdClaim = User.FindFirst("company_id")?.Value;
        return Guid.TryParse(companyIdClaim, out var companyId) ? companyId : null;
    }
}