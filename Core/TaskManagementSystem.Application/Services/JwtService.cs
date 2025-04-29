using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces;

namespace TaskManagementSystem.Application.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtService> _logger;

    public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateToken(UserDTO user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationMinutes"])),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        _logger.LogInformation("Generated JWT token for user with ID: {UserId} at {Time}", user.Id, DateTime.UtcNow);

        return tokenString;
    }

    public bool ValidateToken(string token, out int userId)
    {
        _logger.LogInformation("Start, Validating token...");
        userId = 0; // Default value jika validasi gagal

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Token is null or empty.");
            throw new ArgumentNullException(nameof(token), "Token cannot be null or empty.");
        }

        var jwtSecret = _configuration["JwtSettings:Secret"];
        if (string.IsNullOrEmpty(jwtSecret))
        {
            _logger.LogError("JWT secret is missing in configuration.");
            throw new ArgumentNullException(nameof(jwtSecret), "JWT secret cannot be null or empty.");
        }

        var key = Encoding.UTF8.GetBytes(jwtSecret);
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidAudience = _configuration["JwtSettings:Audience"],
                ClockSkew = TimeSpan.Zero
            };

            var validation = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (int.TryParse(userIdClaim, out int parsedUserId))
            {
                userId = parsedUserId;
                _logger.LogInformation("End, Token validated successfully for user with ID: {UserId}", userId);
                return true;
            }
            else
            {
                _logger.LogWarning("UserId in token is not a valid integer.");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token validation.");
            return false;
        }
    }


}