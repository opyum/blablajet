using EmptyLegs.Core.Entities;
using System.Security.Claims;

namespace EmptyLegs.Core.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, string ipAddress);
    Task<bool> ValidateRefreshTokenAsync(string token);
    string GetUserIdFromToken(string token);
}