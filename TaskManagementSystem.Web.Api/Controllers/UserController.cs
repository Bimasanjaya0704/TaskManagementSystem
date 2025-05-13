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
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;

    public UserController(IUserService userService, ILogger<UserController> logger, IUserContextService userContextService, IMapper mapper)
    {
        _userService = userService;
        _logger = logger;
        _userContextService = userContextService;
        _mapper = mapper;
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        _logger.LogInformation("Start, GetAllUsers");

        var result = await _userService.GetAllUsersAsync();
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
    public async Task<IActionResult> GetUserById(Guid id)
    {
        _logger.LogInformation("Start, GetUserById: {UserId}", id);
        
        var result = await _userService.GetUserByIdAsync(id);
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
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto userUpdateDto)
    {
        _logger.LogInformation("Start, UpdateUser: {UserId}", id);

        var result = await _userService.UpdateUserAsync(id, userUpdateDto);
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
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        _logger.LogInformation("Start, DeleteUser: {UserId}", id);
        
        var result = await _userService.DeleteUserAsync(id);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, DeleteUser - Failed: {ErrorMessage}", result.ErrorMessage);
            return NotFound(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        _logger.LogInformation("End, DeleteUser - Success: User Deleted {UserId}", id);
        return Ok(new ApiResponse<string>(true, "User deleted successfully", null));
    }
    
    [Authorize(Roles = "User,Admin")]
    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        _logger.LogInformation("Start, GetUserByEmail: {Email}", email);
        
        var result = await _userService.GetUserByEmailAsync(email);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, GetUserByEmail - Failed: {ErrorMessage}", result.ErrorMessage);
            return NotFound(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var userResponseDto = _mapper.Map<UserResponseDto>(result.Data);

        _logger.LogInformation("End, GetUserByEmail - Success: Found User with Email {Email}", email);
        return Ok(new ApiResponse<UserResponseDto>(true, "Success", userResponseDto));
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet("username/{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        _logger.LogInformation("Start, GetUserByUsername: {Username}", username);

        var result = await _userService.GetUserByUsernameAsync(username);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, GetUserByUsername - Failed: {ErrorMessage}", result.ErrorMessage);
            return NotFound(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var userResponseDto = _mapper.Map<UserResponseDto>(result.Data);

        _logger.LogInformation("End, GetUserByUsername - Success: Found User with Username {Username}", username);
        return Ok(new ApiResponse<UserResponseDto>(true, "Success", userResponseDto));
    }

}
