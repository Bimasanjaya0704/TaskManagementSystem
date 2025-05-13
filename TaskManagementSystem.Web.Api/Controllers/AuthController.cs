using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces;

namespace TaskManagementSystem.API.Controllers;

[EnableCors]
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
    {
        _logger.LogInformation("Start, Registering user with email: {Email}", createUserDto.Email);

        var result = await _authService.RegisterAsync(createUserDto);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Registration failed for email: {Email}, Reason: {Reason}", createUserDto.Email, result.ErrorMessage);
            return BadRequest(new { result.ErrorType, result.ErrorMessage });
        }

        _logger.LogInformation("End, User registered successfully: {Email}", createUserDto.Email);
        return Ok(result.Data);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        _logger.LogInformation("Start, Login attempt for email: {Email}", loginDto.Email);

        var result = await _authService.LoginAsync(loginDto);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Login failed for email: {Email}, Reason: {Reason}", loginDto.Email, result.ErrorMessage);
            return Unauthorized(new { result.ErrorType, result.ErrorMessage });
        }

        _logger.LogInformation("End, User logged in successfully: {Email}", loginDto.Email);
        return Ok(result.Data);
    }
}