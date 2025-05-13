using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskManagementSystem.Domain.Enum;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<JwtMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            AttachUserToContext(context, token);
        }

        await _next(context);
    }

    private void AttachUserToContext(HttpContext context, string token)
{
    try
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]);

        var validationParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = _configuration["JwtSettings:Issuer"],
            ValidAudience = _configuration["JwtSettings:Audience"],
            ClockSkew = TimeSpan.Zero
        };

        var principal = tokenHandler.ValidateToken(token, validationParams, out _);

        foreach (var claim in principal.Claims)
        {
            _logger.LogInformation("JWT CLAIM: {Type} = {Value}", claim.Type, claim.Value);
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = principal.FindFirst(ClaimTypes.Role);

        if (userIdClaim != null &&
            roleClaim != null &&
            Guid.TryParse(userIdClaim.Value, out Guid userId) &&
            Enum.TryParse(typeof(Role), roleClaim.Value, ignoreCase: true, out var parsedRole) &&
            parsedRole is Role userRole)
        {
            context.Items["UserId"] = userId;
            context.Items["Role"] = userRole;
            _logger.LogInformation("✅ User attached: {UserId} | Role: {Role}", userId, userRole);
        }
        else
        {
            _logger.LogWarning("UserIdClaim or RoleClaim is missing or invalid. RoleClaim: {RoleClaim}", roleClaim?.Value);
        }
    }
    catch (SecurityTokenException ex)
    {
        _logger.LogWarning("Token validation failed: {Message}", ex.Message);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error occurred while validating JWT.");
    }
}

}
