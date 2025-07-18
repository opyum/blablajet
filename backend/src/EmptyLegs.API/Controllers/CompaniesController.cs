using AutoMapper;
using EmptyLegs.Application.DTOs;
using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmptyLegs.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CompaniesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CompaniesController> _logger;

    public CompaniesController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CompaniesController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all companies
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="isVerified">Filter by verification status</param>
    /// <param name="isActive">Filter by active status</param>
    /// <returns>Paginated list of companies</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CompanyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isVerified = null,
        [FromQuery] bool? isActive = null)
    {
        try
        {
            var companies = await _unitOfWork.Companies.GetPagedAsync(page, pageSize);

            // Apply filters if specified
            if (isVerified.HasValue || isActive.HasValue)
            {
                companies = await _unitOfWork.Companies.FindAsync(c => 
                    (!isVerified.HasValue || c.IsVerified == isVerified.Value) &&
                    (!isActive.HasValue || c.IsActive == isActive.Value));
            }

            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            
            _logger.LogInformation("Retrieved {Count} companies for page {Page}", companyDtos.Count(), page);
            return Ok(companyDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving companies");
            return StatusCode(500, new { message = "An error occurred while retrieving companies" });
        }
    }

    /// <summary>
    /// Get company by ID
    /// </summary>
    /// <param name="id">Company ID</param>
    /// <returns>Company details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyDto>> GetCompany(Guid id)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            if (company == null)
            {
                return NotFound(new { message = "Company not found" });
            }

            var companyDto = _mapper.Map<CompanyDto>(company);
            return Ok(companyDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company {CompanyId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the company" });
        }
    }

    /// <summary>
    /// Create new company
    /// </summary>
    /// <param name="createCompanyDto">Company data</param>
    /// <returns>Created company details</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CompanyDto>> CreateCompany([FromBody] CreateCompanyDto createCompanyDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if company with same license already exists
            var existingCompany = await _unitOfWork.Companies
                .FirstOrDefaultAsync(c => c.License == createCompanyDto.License);
            
            if (existingCompany != null)
            {
                return Conflict(new { message = "A company with this license already exists" });
            }

            var company = _mapper.Map<Company>(createCompanyDto);
            company.IsActive = true;
            company.IsVerified = false; // Companies need to be verified by admin
            company.CreatedAt = DateTime.UtcNow;
            company.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Companies.AddAsync(company);
            await _unitOfWork.SaveChangesAsync();

            var companyDto = _mapper.Map<CompanyDto>(company);
            
            _logger.LogInformation("Company {CompanyId} created successfully", company.Id);
            return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, companyDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating company");
            return StatusCode(500, new { message = "An error occurred while creating the company" });
        }
    }

    /// <summary>
    /// Update company
    /// </summary>
    /// <param name="id">Company ID</param>
    /// <param name="updateCompanyDto">Updated company data</param>
    /// <returns>Updated company details</returns>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CompanyDto>> UpdateCompany(Guid id, [FromBody] UpdateCompanyDto updateCompanyDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            if (company == null)
            {
                return NotFound(new { message = "Company not found" });
            }

            // Check authorization - only admins or users from the same company can update
            var currentUserRole = GetCurrentUserRole();
            var currentUserCompanyId = GetCurrentUserCompanyId();
            
            if (currentUserRole != "Admin" && currentUserCompanyId != id)
            {
                return Forbid("You can only update your own company");
            }

            // Update company properties
            if (!string.IsNullOrEmpty(updateCompanyDto.Name))
                company.Name = updateCompanyDto.Name;
            
            if (!string.IsNullOrEmpty(updateCompanyDto.Description))
                company.Description = updateCompanyDto.Description;
            
            if (!string.IsNullOrEmpty(updateCompanyDto.ContactEmail))
                company.ContactEmail = updateCompanyDto.ContactEmail;
            
            if (!string.IsNullOrEmpty(updateCompanyDto.ContactPhone))
                company.ContactPhone = updateCompanyDto.ContactPhone;
            
            if (!string.IsNullOrEmpty(updateCompanyDto.Address))
                company.Address = updateCompanyDto.Address;

            company.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Companies.Update(company);
            await _unitOfWork.SaveChangesAsync();

            var companyDto = _mapper.Map<CompanyDto>(company);
            
            _logger.LogInformation("Company {CompanyId} updated successfully", id);
            return Ok(companyDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company {CompanyId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the company" });
        }
    }

    /// <summary>
    /// Verify company (Admin only)
    /// </summary>
    /// <param name="id">Company ID</param>
    /// <returns>Success status</returns>
    [HttpPatch("{id:guid}/verify")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> VerifyCompany(Guid id)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            if (company == null)
            {
                return NotFound(new { message = "Company not found" });
            }

            company.IsVerified = true;
            company.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Companies.Update(company);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Company {CompanyId} verified", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying company {CompanyId}", id);
            return StatusCode(500, new { message = "An error occurred while verifying the company" });
        }
    }

    /// <summary>
    /// Unverify company (Admin only)
    /// </summary>
    /// <param name="id">Company ID</param>
    /// <returns>Success status</returns>
    [HttpPatch("{id:guid}/unverify")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UnverifyCompany(Guid id)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            if (company == null)
            {
                return NotFound(new { message = "Company not found" });
            }

            company.IsVerified = false;
            company.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Companies.Update(company);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Company {CompanyId} unverified", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unverifying company {CompanyId}", id);
            return StatusCode(500, new { message = "An error occurred while unverifying the company" });
        }
    }

    /// <summary>
    /// Deactivate company (Admin only)
    /// </summary>
    /// <param name="id">Company ID</param>
    /// <returns>Success status</returns>
    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeactivateCompany(Guid id)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            if (company == null)
            {
                return NotFound(new { message = "Company not found" });
            }

            company.IsActive = false;
            company.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Companies.Update(company);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Company {CompanyId} deactivated", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating company {CompanyId}", id);
            return StatusCode(500, new { message = "An error occurred while deactivating the company" });
        }
    }

    /// <summary>
    /// Delete company (Admin only - soft delete)
    /// </summary>
    /// <param name="id">Company ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            if (company == null)
            {
                return NotFound(new { message = "Company not found" });
            }

            await _unitOfWork.Companies.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Company {CompanyId} deleted", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting company {CompanyId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the company" });
        }
    }

    /// <summary>
    /// Get company's aircraft
    /// </summary>
    /// <param name="id">Company ID</param>
    /// <returns>List of company's aircraft</returns>
    [HttpGet("{id:guid}/aircraft")]
    [ProducesResponseType(typeof(IEnumerable<AircraftDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<AircraftDto>>> GetCompanyAircraft(Guid id)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            if (company == null)
            {
                return NotFound(new { message = "Company not found" });
            }

            var aircraft = await _unitOfWork.Aircraft.FindAsync(a => a.CompanyId == id && a.IsActive);
            var aircraftDtos = _mapper.Map<IEnumerable<AircraftDto>>(aircraft);
            
            _logger.LogInformation("Retrieved {Count} aircraft for company {CompanyId}", aircraftDtos.Count(), id);
            return Ok(aircraftDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving aircraft for company {CompanyId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving company aircraft" });
        }
    }

    /// <summary>
    /// Get company's flights
    /// </summary>
    /// <param name="id">Company ID</param>
    /// <param name="status">Filter by flight status</param>
    /// <returns>List of company's flights</returns>
    [HttpGet("{id:guid}/flights")]
    [ProducesResponseType(typeof(IEnumerable<FlightDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<FlightDto>>> GetCompanyFlights(
        Guid id,
        [FromQuery] string? status = null)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            if (company == null)
            {
                return NotFound(new { message = "Company not found" });
            }

            var flights = string.IsNullOrEmpty(status)
                ? await _unitOfWork.Flights.FindAsync(f => f.CompanyId == id)
                : await _unitOfWork.Flights.FindAsync(f => f.CompanyId == id && f.Status.ToString() == status);

            var flightDtos = _mapper.Map<IEnumerable<FlightDto>>(flights);
            
            _logger.LogInformation("Retrieved {Count} flights for company {CompanyId}", flightDtos.Count(), id);
            return Ok(flightDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving flights for company {CompanyId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving company flights" });
        }
    }

    /// <summary>
    /// Get company statistics (Admin or company users only)
    /// </summary>
    /// <param name="id">Company ID</param>
    /// <returns>Company statistics</returns>
    [HttpGet("{id:guid}/statistics")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<object>> GetCompanyStatistics(Guid id)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            if (company == null)
            {
                return NotFound(new { message = "Company not found" });
            }

            // Check authorization - only admins or users from the same company can access
            var currentUserRole = GetCurrentUserRole();
            var currentUserCompanyId = GetCurrentUserCompanyId();
            
            if (currentUserRole != "Admin" && currentUserCompanyId != id)
            {
                return Forbid("You can only access your own company statistics");
            }

            var totalAircraft = await _unitOfWork.Aircraft.CountAsync(a => a.CompanyId == id);
            var totalFlights = await _unitOfWork.Flights.CountAsync(f => f.CompanyId == id);
            var totalBookings = await _unitOfWork.Bookings.CountAsync(b => b.Flight.CompanyId == id);

            var statistics = new
            {
                TotalAircraft = totalAircraft,
                ActiveAircraft = await _unitOfWork.Aircraft.CountAsync(a => a.CompanyId == id && a.IsActive),
                TotalFlights = totalFlights,
                AvailableFlights = await _unitOfWork.Flights.CountAsync(f => f.CompanyId == id && f.Status == Core.Enums.FlightStatus.Available),
                TotalBookings = totalBookings,
                Revenue = new
                {
                    ThisMonth = await CalculateMonthlyRevenue(id),
                    LastMonth = await CalculateMonthlyRevenue(id, -1),
                    ThisYear = await CalculateYearlyRevenue(id)
                },
                AverageRating = company.AverageRating,
                TotalReviews = company.TotalReviews
            };

            _logger.LogInformation("Company statistics retrieved for {CompanyId}", id);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statistics for company {CompanyId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving company statistics" });
        }
    }

    private async Task<decimal> CalculateMonthlyRevenue(Guid companyId, int monthOffset = 0)
    {
        var targetMonth = DateTime.UtcNow.AddMonths(monthOffset);
        var startOfMonth = new DateTime(targetMonth.Year, targetMonth.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        var bookings = await _unitOfWork.Bookings.FindAsync(b => 
            b.Flight.CompanyId == companyId &&
            b.BookingDate >= startOfMonth &&
            b.BookingDate < endOfMonth &&
            (b.Status == Core.Enums.BookingStatus.Confirmed || b.Status == Core.Enums.BookingStatus.Completed));

        return bookings.Sum(b => b.TotalAmount);
    }

    private async Task<decimal> CalculateYearlyRevenue(Guid companyId)
    {
        var startOfYear = new DateTime(DateTime.UtcNow.Year, 1, 1);
        var endOfYear = startOfYear.AddYears(1);

        var bookings = await _unitOfWork.Bookings.FindAsync(b => 
            b.Flight.CompanyId == companyId &&
            b.BookingDate >= startOfYear &&
            b.BookingDate < endOfYear &&
            (b.Status == Core.Enums.BookingStatus.Confirmed || b.Status == Core.Enums.BookingStatus.Completed));

        return bookings.Sum(b => b.TotalAmount);
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