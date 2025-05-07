using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TaskManagementSystem.Web.Api.Services;

public class TokenService
{
    private readonly ILogger<TokenService> _logger;

    public TokenService(ILogger<TokenService> logger)
    {
        _logger = logger;
    }

    public string GetTokenFromHeader(HttpRequest request)
    {
        var authorizationHeader = request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            _logger.LogWarning("Authorization header is missing or invalid.");
            return null;
        }
        _logger.LogInformation("Authorization header received: {Header}", authorizationHeader);
        return authorizationHeader.Split(" ").Last();
    }
    
    public Guid GetUserIdFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier || c.Type == "userId" || c.Type == "sub");

            if (userIdClaim == null || string.IsNullOrWhiteSpace(userIdClaim.Value))
            {
                _logger.LogWarning("User ID claim not found in token.");
                return Guid.Empty;
            }

            return Guid.TryParse(userIdClaim.Value, out var userId) ? userId : Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse JWT token.");
            return Guid.Empty;
        }
    }
}

