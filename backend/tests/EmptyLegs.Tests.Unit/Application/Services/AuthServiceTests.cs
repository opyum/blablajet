using AutoMapper;
using EmptyLegs.Application.DTOs;
using EmptyLegs.Application.Services;
using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Enums;
using EmptyLegs.Core.Interfaces;
using EmptyLegs.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace EmptyLegs.Tests.Unit.Application.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockJwtTokenService = new Mock<IJwtTokenService>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<AuthService>>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        _authService = new AuthService(
            _mockUnitOfWork.Object,
            _mockJwtTokenService.Object,
            _mockMapper.Object,
            _mockLogger.Object,
            _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = HashPassword("password123"), // Using a simple hash for testing
            IsActive = true,
            Role = UserRole.Customer
        };

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        var refreshToken = new RefreshToken
        {
            Token = "refresh_token_123",
            UserId = user.Id
        };

        _mockUnitOfWork.Setup(x => x.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), default))
            .ReturnsAsync(user);
        
        _mockJwtTokenService.Setup(x => x.GenerateAccessToken(user))
            .Returns("access_token_123");
        
        _mockJwtTokenService.Setup(x => x.CreateRefreshTokenAsync(user.Id, It.IsAny<string>()))
            .ReturnsAsync(refreshToken);
        
        _mockMapper.Setup(x => x.Map<UserDto>(user))
            .Returns(userDto);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("access_token_123", result.AccessToken);
        Assert.Equal("refresh_token_123", result.RefreshToken);
        Assert.Equal(15 * 60, result.ExpiresIn);
        Assert.Equal(userDto, result.User);

        _mockUnitOfWork.Verify(x => x.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), default), Times.Once);
        _mockJwtTokenService.Verify(x => x.GenerateAccessToken(user), Times.Once);
        _mockJwtTokenService.Verify(x => x.CreateRefreshTokenAsync(user.Id, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_InvalidEmail_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };

        _mockUnitOfWork.Setup(x => x.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), default))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = HashPassword("correctpassword"),
            IsActive = true
        };

        _mockUnitOfWork.Setup(x => x.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), default))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task RegisterAsync_ValidData_ReturnsAuthResponse()
    {
        // Arrange
        var registerDto = new RegisterUserDto
        {
            Email = "newuser@example.com",
            Password = "password123",
            FirstName = "Jane",
            LastName = "Doe",
            Role = UserRole.Customer
        };

        var userDto = new UserDto
        {
            Id = Guid.NewGuid(),
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName
        };

        var refreshToken = new RefreshToken
        {
            Token = "refresh_token_123"
        };

        _mockUnitOfWork.Setup(x => x.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), default))
            .ReturnsAsync((User?)null); // No existing user

        _mockUnitOfWork.Setup(x => x.Users.AddAsync(It.IsAny<User>(), default))
            .ReturnsAsync((User user) => user);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<User>()))
            .Returns("access_token_123");

        _mockJwtTokenService.Setup(x => x.CreateRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(refreshToken);

        _mockMapper.Setup(x => x.Map<UserDto>(It.IsAny<User>()))
            .Returns(userDto);

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("access_token_123", result.AccessToken);
        Assert.Equal("refresh_token_123", result.RefreshToken);
        Assert.Equal(userDto, result.User);

        _mockUnitOfWork.Verify(x => x.Users.AddAsync(It.IsAny<User>(), default), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ExistingEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var registerDto = new RegisterUserDto
        {
            Email = "existing@example.com",
            Password = "password123",
            FirstName = "Jane",
            LastName = "Doe"
        };

        var existingUser = new User { Email = "existing@example.com" };

        _mockUnitOfWork.Setup(x => x.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), default))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RegisterAsync(registerDto));
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidToken_ReturnsAuthResponse()
    {
        // Arrange
        var refreshTokenValue = "valid_refresh_token";
        var userId = Guid.NewGuid();
        
        var storedRefreshToken = new RefreshToken
        {
            Token = refreshTokenValue,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            IsRevoked = false
        };

        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true
        };

        var userDto = new UserDto
        {
            Id = userId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        var newRefreshToken = new RefreshToken
        {
            Token = "new_refresh_token",
            UserId = userId
        };

        _mockUnitOfWork.Setup(x => x.RefreshTokens.FirstOrDefaultAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>(), default))
            .ReturnsAsync(storedRefreshToken);

        _mockUnitOfWork.Setup(x => x.Users.GetByIdAsync(userId, default))
            .ReturnsAsync(user);

        _mockJwtTokenService.Setup(x => x.GenerateAccessToken(user))
            .Returns("new_access_token");

        _mockJwtTokenService.Setup(x => x.CreateRefreshTokenAsync(userId, It.IsAny<string>()))
            .ReturnsAsync(newRefreshToken);

        _mockMapper.Setup(x => x.Map<UserDto>(user))
            .Returns(userDto);

        // Act
        var result = await _authService.RefreshTokenAsync(refreshTokenValue);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new_access_token", result.AccessToken);
        Assert.Equal("new_refresh_token", result.RefreshToken);
        Assert.Equal(userDto, result.User);

        // Verify that old token was revoked
        Assert.True(storedRefreshToken.IsRevoked);
        Assert.Equal("new_refresh_token", storedRefreshToken.ReplacedByToken);

        _mockUnitOfWork.Verify(x => x.RefreshTokens.Update(storedRefreshToken), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_InvalidToken_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var refreshTokenValue = "invalid_refresh_token";

        _mockUnitOfWork.Setup(x => x.RefreshTokens.FirstOrDefaultAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>(), default))
            .ReturnsAsync((RefreshToken?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.RefreshTokenAsync(refreshTokenValue));
    }

    [Fact]
    public async Task RevokeTokenAsync_ValidToken_ReturnsTrue()
    {
        // Arrange
        var refreshTokenValue = "valid_refresh_token";
        var refreshToken = new RefreshToken
        {
            Token = refreshTokenValue,
            UserId = Guid.NewGuid(),
            IsRevoked = false
        };

        _mockUnitOfWork.Setup(x => x.RefreshTokens.FirstOrDefaultAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>(), default))
            .ReturnsAsync(refreshToken);

        // Act
        var result = await _authService.RevokeTokenAsync(refreshTokenValue);

        // Assert
        Assert.True(result);
        Assert.True(refreshToken.IsRevoked);
        Assert.NotNull(refreshToken.RevokedAt);

        _mockUnitOfWork.Verify(x => x.RefreshTokens.Update(refreshToken), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task RevokeTokenAsync_TokenNotFound_ReturnsFalse()
    {
        // Arrange
        var refreshTokenValue = "nonexistent_token";

        _mockUnitOfWork.Setup(x => x.RefreshTokens.FirstOrDefaultAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>(), default))
            .ReturnsAsync((RefreshToken?)null);

        // Act
        var result = await _authService.RevokeTokenAsync(refreshTokenValue);

        // Assert
        Assert.False(result);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + "EmptyLegs_Salt"));
        return Convert.ToBase64String(hashedBytes);
    }
}