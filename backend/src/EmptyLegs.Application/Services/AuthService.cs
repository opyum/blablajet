using AutoMapper;
using EmptyLegs.Application.DTOs;
using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Enums;
using EmptyLegs.Core.Interfaces;
using EmptyLegs.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace EmptyLegs.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        IMapper mapper,
        ILogger<AuthService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        try
        {
            // Find user by email
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Login attempt failed for email: {Email}", loginDto.Email);
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Verify password
            if (!VerifyPassword(loginDto.Password, user.PasswordHash!))
            {
                _logger.LogWarning("Invalid password attempt for user: {UserId}", user.Id);
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var ipAddress = GetIpAddress();
            var refreshToken = await _jwtTokenService.CreateRefreshTokenAsync(user.Id, ipAddress);

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn = 15 * 60, // 15 minutes in seconds
                User = _mapper.Map<UserDto>(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", loginDto.Email);
            throw;
        }
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with this email already exists");
            }

            // Create new user
            var user = new User
            {
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber,
                DateOfBirth = registerDto.DateOfBirth,
                Role = registerDto.Role,
                PasswordHash = HashPassword(registerDto.Password),
                IsActive = true,
                IsEmailVerified = false // Will be verified later
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var ipAddress = GetIpAddress();
            var refreshToken = await _jwtTokenService.CreateRefreshTokenAsync(user.Id, ipAddress);

            _logger.LogInformation("New user registered: {UserId}", user.Id);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn = 15 * 60, // 15 minutes in seconds
                User = _mapper.Map<UserDto>(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", registerDto.Email);
            throw;
        }
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            // Validate refresh token
            var storedRefreshToken = await _unitOfWork.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedRefreshToken == null || !storedRefreshToken.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            // Get user
            var user = await _unitOfWork.Users.GetByIdAsync(storedRefreshToken.UserId);
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("User not found or inactive");
            }

            // Revoke old refresh token
            storedRefreshToken.IsRevoked = true;
            storedRefreshToken.RevokedAt = DateTime.UtcNow;
            storedRefreshToken.RevokedByIp = GetIpAddress();

            // Generate new tokens
            var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
            var ipAddress = GetIpAddress();
            var newRefreshToken = await _jwtTokenService.CreateRefreshTokenAsync(user.Id, ipAddress);

            // Link old token to new one
            storedRefreshToken.ReplacedByToken = newRefreshToken.Token;
            
            _unitOfWork.RefreshTokens.Update(storedRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Token refreshed for user: {UserId}", user.Id);

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresIn = 15 * 60, // 15 minutes in seconds
                User = _mapper.Map<UserDto>(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            throw;
        }
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        try
        {
            var token = await _unitOfWork.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null)
                return false;

            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = GetIpAddress();

            _unitOfWork.RefreshTokens.Update(token);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Refresh token revoked for user: {UserId}", token.UserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token");
            return false;
        }
    }

    public async Task<bool> RevokeAllTokensAsync(Guid userId)
    {
        try
        {
            var tokens = await _unitOfWork.RefreshTokens
                .FindAsync(rt => rt.UserId == userId && !rt.IsRevoked);

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = GetIpAddress();
            }

            _unitOfWork.RefreshTokens.UpdateRange(tokens);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("All refresh tokens revoked for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking all tokens for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            return await _jwtTokenService.ValidateRefreshTokenAsync(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Verify current password
            if (!VerifyPassword(currentPassword, user.PasswordHash!))
                return false;

            // Update password
            user.PasswordHash = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Revoke all refresh tokens to force re-login
            await RevokeAllTokensAsync(userId);

            _logger.LogInformation("Password changed for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(string email)
    {
        try
        {
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                // Don't reveal that user doesn't exist
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
                return true; // Return true to not reveal user existence
            }

            // In a real implementation, you would:
            // 1. Generate a password reset token
            // 2. Store it in database with expiration
            // 3. Send email with reset link
            
            _logger.LogInformation("Password reset requested for user: {UserId}", user.Id);
            
            // TODO: Implement email service and password reset logic
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset for email: {Email}", email);
            return false;
        }
    }

    public async Task<bool> ConfirmEmailAsync(Guid userId, string token)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return false;

            // TODO: Implement token validation logic
            user.IsEmailVerified = true;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Email confirmed for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming email for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> ResendEmailConfirmationAsync(string email)
    {
        try
        {
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.IsEmailVerified)
                return false;

            // TODO: Implement email confirmation resend logic
            _logger.LogInformation("Email confirmation resent for user: {UserId}", user.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resending email confirmation for email: {Email}", email);
            return false;
        }
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "EmptyLegs_Salt"));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        var hashToCompare = HashPassword(password);
        return hashToCompare == hashedPassword;
    }

    private string GetIpAddress()
    {
        if (_httpContextAccessor.HttpContext?.Request.Headers.ContainsKey("X-Forwarded-For") == true)
        {
            return _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].ToString();
        }
        
        return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}