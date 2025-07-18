using EmptyLegs.Application.DTOs;
using EmptyLegs.Core.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace EmptyLegs.Tests.Integration.Controllers;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidData_ReturnsCreatedWithTokens()
    {
        // Arrange
        var registerDto = new RegisterUserDto
        {
            Email = $"test{Guid.NewGuid()}@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(authResponse);
        Assert.NotEmpty(authResponse.AccessToken);
        Assert.NotEmpty(authResponse.RefreshToken);
        Assert.Equal(15 * 60, authResponse.ExpiresIn);
        Assert.NotNull(authResponse.User);
        Assert.Equal(registerDto.Email, authResponse.User.Email);
        Assert.Equal(registerDto.FirstName, authResponse.User.FirstName);
        Assert.Equal(registerDto.LastName, authResponse.User.LastName);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var email = $"duplicate{Guid.NewGuid()}@example.com";
        var registerDto = new RegisterUserDto
        {
            Email = email,
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer
        };

        // Register the user first time
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);

        // Act - Try to register again with same email
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithTokens()
    {
        // Arrange - First register a user
        var email = $"login{Guid.NewGuid()}@example.com";
        var password = "Password123!";
        var registerDto = new RegisterUserDto
        {
            Email = email,
            Password = password,
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer
        };
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);

        var loginDto = new LoginDto
        {
            Email = email,
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(authResponse);
        Assert.NotEmpty(authResponse.AccessToken);
        Assert.NotEmpty(authResponse.RefreshToken);
        Assert.Equal(email, authResponse.User.Email);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_ValidToken_ReturnsOkWithNewTokens()
    {
        // Arrange - First register and login a user
        var email = $"refresh{Guid.NewGuid()}@example.com";
        var password = "Password123!";
        var registerDto = new RegisterUserDto
        {
            Email = email,
            Password = password,
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer
        };
        
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);
        var registerContent = await registerResponse.Content.ReadAsStringAsync();
        var registerAuthResponse = JsonSerializer.Deserialize<AuthResponseDto>(registerContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var refreshRequest = new RefreshTokenDto
        {
            RefreshToken = registerAuthResponse!.RefreshToken
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(authResponse);
        Assert.NotEmpty(authResponse.AccessToken);
        Assert.NotEmpty(authResponse.RefreshToken);
        Assert.NotEqual(registerAuthResponse.AccessToken, authResponse.AccessToken);
        Assert.NotEqual(registerAuthResponse.RefreshToken, authResponse.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var refreshRequest = new RefreshTokenDto
        {
            RefreshToken = "invalid_refresh_token"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_WithValidToken_ReturnsUserInfo()
    {
        // Arrange - First register a user
        var email = $"profile{Guid.NewGuid()}@example.com";
        var password = "Password123!";
        var registerDto = new RegisterUserDto
        {
            Email = email,
            Password = password,
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer
        };
        
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);
        var registerContent = await registerResponse.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(registerContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Add the access token to the request
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse!.AccessToken);

        // Act
        var response = await _client.GetAsync("/api/v1/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<JsonElement>(responseContent);

        Assert.Equal(email, userInfo.GetProperty("email").GetString());
        Assert.Equal("John Doe", userInfo.GetProperty("name").GetString());
        Assert.Equal("Customer", userInfo.GetProperty("role").GetString());
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RevokeToken_WithValidToken_ReturnsOk()
    {
        // Arrange - First register a user
        var email = $"revoke{Guid.NewGuid()}@example.com";
        var password = "Password123!";
        var registerDto = new RegisterUserDto
        {
            Email = email,
            Password = password,
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer
        };
        
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);
        var registerContent = await registerResponse.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(registerContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Add the access token to the request
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse!.AccessToken);

        var revokeRequest = new RefreshTokenDto
        {
            RefreshToken = authResponse.RefreshToken
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/revoke", revokeRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WithValidEmail_ReturnsOk()
    {
        // Arrange
        var resetRequest = new ResetPasswordDto
        {
            Email = "test@example.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/reset-password", resetRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        Assert.Contains("reset link", result.GetProperty("message").GetString()!.ToLower());
    }
}