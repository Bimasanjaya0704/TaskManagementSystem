using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Web.Api.DTOs;
using TaskManagementSystem.Web.Api.Models;
using TaskManagementSystem.Web.Api.Services;

namespace TaskManagementSystem.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;
    private readonly TokenService _tokenService;
    private readonly IMapper _mapper;

    public UserController(IUserService userService, ILogger<UserController> logger, TokenService tokenService, IMapper mapper)
    {
        _userService = userService;
        _logger = logger;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        _logger.LogInformation("Start, GetAllUsers");

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("End, GetAllUsers - Failed: Token is missing");
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var result = await _userService.GetAllAsync(token);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, GetAllUsers - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var userResponseDto = _mapper.Map<List<UserResponseDto>>(result.Data);

        _logger.LogInformation("End, GetAllUsers - Success, Retrieved {Count} users", userResponseDto.Count);
        return Ok(new ApiResponse<List<UserResponseDto>>(true, "Success", userResponseDto));
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        _logger.LogInformation("Start, GetUserById: {UserId}", id);

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("End, GetUserById - Failed: Token is missing");
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var result = await _userService.GetByIdAsync(id, token);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, GetUserById - Failed: {ErrorMessage}", result.ErrorMessage);
            return NotFound(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var userResponseDto = _mapper.Map<UserResponseDto>(result.Data);

        _logger.LogInformation("End, GetUserById - Success: Found User {UserId}", id);
        return Ok(new ApiResponse<UserResponseDto>(true, "Success", userResponseDto));
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userUpdateDto)
    {
        _logger.LogInformation("Start, UpdateUser: {UserId}", id);

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("End, UpdateUser - Failed: Token is missing");
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var result = await _userService.UpdateAsync(id, token, userUpdateDto);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, UpdateUser - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var updatedUser = result.Data;
        var userResponseDto = _mapper.Map<UserResponseDto>(updatedUser);

        _logger.LogInformation("End, UpdateUser - Success: User Updated {UserId}", id);
        return Ok(new ApiResponse<UserResponseDto>(true, "User updated successfully", userResponseDto));
    }

    [Authorize(Roles = "User,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        _logger.LogInformation("Start, DeleteUser: {UserId}", id);

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("End, DeleteUser - Failed: Token is missing");
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var result = await _userService.DeleteAsync(id, token);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, DeleteUser - Failed: {ErrorMessage}", result.ErrorMessage);
            return NotFound(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        _logger.LogInformation("End, DeleteUser - Success: User Deleted {UserId}", id);
        return Ok(new ApiResponse<string>(true, "User deleted successfully", null));
    }
}
