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
}

