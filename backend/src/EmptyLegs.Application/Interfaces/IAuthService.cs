using EmptyLegs.Application.DTOs;

namespace EmptyLegs.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken);
    Task<bool> RevokeAllTokensAsync(Guid userId);
    Task<bool> ValidateTokenAsync(string token);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<bool> ResetPasswordAsync(string email);
    Task<bool> ConfirmEmailAsync(Guid userId, string token);
    Task<bool> ResendEmailConfirmationAsync(string email);
}