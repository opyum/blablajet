using AutoMapper;
using EmptyLegs.Application.DTOs;
using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmptyLegs.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AirportsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AirportsController> _logger;

    public AirportsController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AirportsController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all airports
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="country">Filter by country</param>
    /// <param name="city">Filter by city</param>
    /// <param name="search">Search in airport name, city, or IATA code</param>
    /// <param name="isActive">Filter by active status</param>
    /// <returns>Paginated list of airports</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AirportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AirportDto>>> GetAirports(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? country = null,
        [FromQuery] string? city = null,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = true)
    {
        try
        {
            IEnumerable<Airport> airports;

            if (!string.IsNullOrEmpty(search) || !string.IsNullOrEmpty(country) || !string.IsNullOrEmpty(city) || isActive.HasValue)
            {
                airports = await _unitOfWork.Airports.FindAsync(a =>
                    (string.IsNullOrEmpty(search) || 
                     a.Name.Contains(search) || 
                     a.City.Contains(search) || 
                     a.IataCode.Contains(search) ||
                     a.IcaoCode.Contains(search)) &&
                    (string.IsNullOrEmpty(country) || a.Country.Contains(country)) &&
                    (string.IsNullOrEmpty(city) || a.City.Contains(city)) &&
                    (!isActive.HasValue || a.IsActive == isActive.Value));
            }
            else
            {
                airports = await _unitOfWork.Airports.GetPagedAsync(page, pageSize);
            }

            var airportDtos = _mapper.Map<IEnumerable<AirportDto>>(airports);
            
            _logger.LogInformation("Retrieved {Count} airports for page {Page}", airportDtos.Count(), page);
            return Ok(airportDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving airports");
            return StatusCode(500, new { message = "An error occurred while retrieving airports" });
        }
    }

    /// <summary>
    /// Get airport by ID
    /// </summary>
    /// <param name="id">Airport ID</param>
    /// <returns>Airport details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AirportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AirportDto>> GetAirport(Guid id)
    {
        try
        {
            var airport = await _unitOfWork.Airports.GetByIdAsync(id);
            if (airport == null)
            {
                return NotFound(new { message = "Airport not found" });
            }

            var airportDto = _mapper.Map<AirportDto>(airport);
            return Ok(airportDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving airport {AirportId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the airport" });
        }
    }

    /// <summary>
    /// Get airport by IATA code
    /// </summary>
    /// <param name="iataCode">Airport IATA code (e.g., CDG, JFK)</param>
    /// <returns>Airport details</returns>
    [HttpGet("iata/{iataCode}")]
    [ProducesResponseType(typeof(AirportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AirportDto>> GetAirportByIata(string iataCode)
    {
        try
        {
            var airport = await _unitOfWork.Airports.FirstOrDefaultAsync(a => a.IataCode == iataCode.ToUpper());
            if (airport == null)
            {
                return NotFound(new { message = "Airport not found" });
            }

            var airportDto = _mapper.Map<AirportDto>(airport);
            return Ok(airportDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving airport with IATA code {IataCode}", iataCode);
            return StatusCode(500, new { message = "An error occurred while retrieving the airport" });
        }
    }

    /// <summary>
    /// Search airports by text
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="limit">Maximum number of results</param>
    /// <returns>List of matching airports</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<AirportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AirportDto>>> SearchAirports(
        [FromQuery] string query,
        [FromQuery] int limit = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { message = "Search query is required" });
            }

            var airports = await _unitOfWork.Airports.FindAsync(a =>
                a.IsActive &&
                (a.Name.Contains(query) ||
                 a.City.Contains(query) ||
                 a.Country.Contains(query) ||
                 a.IataCode.Contains(query.ToUpper()) ||
                 a.IcaoCode.Contains(query.ToUpper())));

            var limitedAirports = airports.Take(limit);
            var airportDtos = _mapper.Map<IEnumerable<AirportDto>>(limitedAirports);
            
            _logger.LogInformation("Found {Count} airports matching query '{Query}'", airportDtos.Count(), query);
            return Ok(airportDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching airports with query '{Query}'", query);
            return StatusCode(500, new { message = "An error occurred while searching airports" });
        }
    }

    /// <summary>
    /// Find airports near coordinates
    /// </summary>
    /// <param name="latitude">Latitude</param>
    /// <param name="longitude">Longitude</param>
    /// <param name="radiusKm">Search radius in kilometers</param>
    /// <param name="limit">Maximum number of results</param>
    /// <returns>List of nearby airports</returns>
    [HttpGet("nearby")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<object>>> GetNearbyAirports(
        [FromQuery] decimal latitude,
        [FromQuery] decimal longitude,
        [FromQuery] int radiusKm = 100,
        [FromQuery] int limit = 10)
    {
        try
        {
            var airports = await _unitOfWork.Airports.FindAsync(a => a.IsActive);
            
            var nearbyAirports = airports
                .Select(a => new
                {
                    Airport = a,
                    Distance = CalculateDistance(latitude, longitude, a.Latitude, a.Longitude)
                })
                .Where(x => x.Distance <= radiusKm)
                .OrderBy(x => x.Distance)
                .Take(limit)
                .Select(x => new
                {
                    Airport = _mapper.Map<AirportDto>(x.Airport),
                    DistanceKm = Math.Round(x.Distance, 2)
                });

            _logger.LogInformation("Found {Count} airports within {RadiusKm}km of coordinates ({Latitude}, {Longitude})", 
                nearbyAirports.Count(), radiusKm, latitude, longitude);
            
            return Ok(nearbyAirports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding nearby airports");
            return StatusCode(500, new { message = "An error occurred while finding nearby airports" });
        }
    }

    /// <summary>
    /// Create new airport (Admin only)
    /// </summary>
    /// <param name="createAirportDto">Airport data</param>
    /// <returns>Created airport details</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AirportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AirportDto>> CreateAirport([FromBody] CreateAirportDto createAirportDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if airport with same IATA code already exists
            var existingAirport = await _unitOfWork.Airports
                .FirstOrDefaultAsync(a => a.IataCode == createAirportDto.IataCode.ToUpper());
            
            if (existingAirport != null)
            {
                return Conflict(new { message = "An airport with this IATA code already exists" });
            }

            var airport = _mapper.Map<Airport>(createAirportDto);
            airport.IataCode = createAirportDto.IataCode.ToUpper();
            airport.IcaoCode = createAirportDto.IcaoCode.ToUpper();
            airport.IsActive = true;
            airport.CreatedAt = DateTime.UtcNow;
            airport.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Airports.AddAsync(airport);
            await _unitOfWork.SaveChangesAsync();

            var airportDto = _mapper.Map<AirportDto>(airport);
            
            _logger.LogInformation("Airport {AirportId} ({IataCode}) created successfully", airport.Id, airport.IataCode);
            return CreatedAtAction(nameof(GetAirport), new { id = airport.Id }, airportDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating airport");
            return StatusCode(500, new { message = "An error occurred while creating the airport" });
        }
    }

    /// <summary>
    /// Update airport (Admin only)
    /// </summary>
    /// <param name="id">Airport ID</param>
    /// <param name="updateAirportDto">Updated airport data</param>
    /// <returns>Updated airport details</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AirportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AirportDto>> UpdateAirport(Guid id, [FromBody] CreateAirportDto updateAirportDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var airport = await _unitOfWork.Airports.GetByIdAsync(id);
            if (airport == null)
            {
                return NotFound(new { message = "Airport not found" });
            }

            // Update airport properties
            airport.Name = updateAirportDto.Name;
            airport.City = updateAirportDto.City;
            airport.Country = updateAirportDto.Country;
            airport.Latitude = updateAirportDto.Latitude;
            airport.Longitude = updateAirportDto.Longitude;
            airport.TimeZone = updateAirportDto.TimeZone;
            airport.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Airports.Update(airport);
            await _unitOfWork.SaveChangesAsync();

            var airportDto = _mapper.Map<AirportDto>(airport);
            
            _logger.LogInformation("Airport {AirportId} updated successfully", id);
            return Ok(airportDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating airport {AirportId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the airport" });
        }
    }

    /// <summary>
    /// Deactivate airport (Admin only)
    /// </summary>
    /// <param name="id">Airport ID</param>
    /// <returns>Success status</returns>
    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeactivateAirport(Guid id)
    {
        try
        {
            var airport = await _unitOfWork.Airports.GetByIdAsync(id);
            if (airport == null)
            {
                return NotFound(new { message = "Airport not found" });
            }

            airport.IsActive = false;
            airport.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Airports.Update(airport);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Airport {AirportId} deactivated", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating airport {AirportId}", id);
            return StatusCode(500, new { message = "An error occurred while deactivating the airport" });
        }
    }

    /// <summary>
    /// Delete airport (Admin only - soft delete)
    /// </summary>
    /// <param name="id">Airport ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteAirport(Guid id)
    {
        try
        {
            var airport = await _unitOfWork.Airports.GetByIdAsync(id);
            if (airport == null)
            {
                return NotFound(new { message = "Airport not found" });
            }

            await _unitOfWork.Airports.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Airport {AirportId} deleted", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting airport {AirportId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the airport" });
        }
    }

    /// <summary>
    /// Get countries with airports
    /// </summary>
    /// <returns>List of countries</returns>
    [HttpGet("countries")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetCountries()
    {
        try
        {
            var airports = await _unitOfWork.Airports.FindAsync(a => a.IsActive);
            var countries = airports
                .Select(a => a.Country)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            _logger.LogInformation("Retrieved {Count} countries with airports", countries.Count);
            return Ok(countries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving countries");
            return StatusCode(500, new { message = "An error occurred while retrieving countries" });
        }
    }

    /// <summary>
    /// Get cities in a country with airports
    /// </summary>
    /// <param name="country">Country name</param>
    /// <returns>List of cities</returns>
    [HttpGet("countries/{country}/cities")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetCitiesInCountry(string country)
    {
        try
        {
            var airports = await _unitOfWork.Airports.FindAsync(a => a.IsActive && a.Country == country);
            var cities = airports
                .Select(a => a.City)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            _logger.LogInformation("Retrieved {Count} cities in {Country} with airports", cities.Count, country);
            return Ok(cities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cities for country {Country}", country);
            return StatusCode(500, new { message = "An error occurred while retrieving cities" });
        }
    }

    /// <summary>
    /// Calculate distance between two points using Haversine formula
    /// </summary>
    private static double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        const double earthRadiusKm = 6371.0;

        var dLat = DegreesToRadians((double)(lat2 - lat1));
        var dLon = DegreesToRadians((double)(lon2 - lon1));

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians((double)lat1)) * Math.Cos(DegreesToRadians((double)lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}