using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Enum;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Result;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;

namespace TaskManagementSystem.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    private readonly IJwtService _jwtService;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _jwtService = jwtService;
    }

    private bool ValidateToken(string token, out int userId)
    {
        return _jwtService.ValidateToken(token, out userId);
    }

    public async Task<TaskErrorResult<UserDTO>> GetByIdAsync(int id, string token)
    {
        _logger.LogInformation("Start, Fetching user with ID: {UserId}", id);

        if (!ValidateToken(token, out int userId))
        {
            _logger.LogWarning("Invalid or expired token.");
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorInvalidCredentials, "Invalid token.");
        }

        if (id <= 0 || userId != id)
        {
            _logger.LogWarning("Invalid user ID: {UserId}", id);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid user ID.");
        }

        var userEntity = await _unitOfWork.UserRepository.GetUserByIdAsync(id);
        if (userEntity == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", id);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorUserNotFound, "User not found.");
        }

        var userDto = _mapper.Map<UserDTO>(userEntity);
        _logger.LogInformation("End, Fetching user with ID: {UserId}", id);
        return TaskErrorResult<UserDTO>.Success(userDto);
    }

    public async Task<TaskErrorResult<IEnumerable<UserDTO>>> GetAllAsync(string token)
    {
        _logger.LogInformation("Start, Fetching all users");

        if (!ValidateToken(token, out int userId))
        {
            _logger.LogWarning("Invalid or expired token.");
            return TaskErrorResult<IEnumerable<UserDTO>>.Failure(TaskErrorType.ErrorInvalidCredentials, "Invalid token.");
        }

        if (userId <= 0)
        {
            _logger.LogWarning("Invalid user ID: {UserId}", userId);
            return TaskErrorResult<IEnumerable<UserDTO>>.Failure(TaskErrorType.ErrorInvalidId, "Invalid user ID.");
        }


        var userEntities = await _unitOfWork.UserRepository.GetAllUserAsync();

        if (userEntities == null || !userEntities.Any())
        {
            _logger.LogWarning("No users found.");
            return TaskErrorResult<IEnumerable<UserDTO>>.Failure(TaskErrorType.ErrorUserNotFound, "No users found.");
        }

        var userDtos = _mapper.Map<IEnumerable<UserDTO>>(userEntities);
        _logger.LogInformation("End, Fetching all users");
        return TaskErrorResult<IEnumerable<UserDTO>>.Success(userDtos);
    }

    public async Task<TaskErrorResult<UserDTO>> UpdateAsync(int id, string token, UserDTO userDto)
    {
        _logger.LogInformation("Start, Updating user with ID: {UserId}", id);

        if (!ValidateToken(token, out int userId))
        {
            _logger.LogWarning("Invalid or expired token.");
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorInvalidCredentials, "Invalid token.");
        }

        if (id <= 0 || userId != id || userDto.Id != 0 && userDto.Id != id)
        {
            _logger.LogWarning("Invalid user ID: {UserId}", id);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid user ID.");
        }

        if (userDto.Id == null)
        {
            _logger.LogWarning("User data cannot be null");
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorUpdateUser, "User data null.");
        }

        var existingUser = await _unitOfWork.UserRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", id);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorUserNotFound, "User not found.");
        }

        _mapper.Map(userDto, existingUser);
        await _unitOfWork.UserRepository.UpdateUserAsync(id, existingUser);
        await _unitOfWork.CommitAsync();

        var updatedUserDto = _mapper.Map<UserDTO>(existingUser);
        _logger.LogInformation("End, Updating user with ID: {UserId}", id);
        return TaskErrorResult<UserDTO>.Success(updatedUserDto);
    }

    public async Task<TaskErrorResult<UserDTO>> DeleteAsync(int id, string token)
    {
        _logger.LogInformation("Start, Deleting user with ID: {UserId}", id);

        if (!ValidateToken(token, out int userId))
        {
            _logger.LogWarning("Invalid or expired token.");
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorInvalidCredentials, "Invalid token.");
        }

        if (id <= 0 || userId != id)
        {
            _logger.LogWarning("Invalid user ID: {UserId}", id);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid user ID.");
        }

        var existingUser = await _unitOfWork.UserRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", id);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorUserNotFound, "User not found.");
        }

        var deleted = await _unitOfWork.UserRepository.DeleteUserAsync(id);
        if (!deleted)
        {
            _logger.LogError("Failed to delete user with ID: {UserId}", id);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorDeleteUser, "Failed to delete user.");
        }

        await _unitOfWork.CommitAsync();
        _logger.LogInformation("End, Deleting user with ID: {UserId}", id);
        return TaskErrorResult<UserDTO>.Success();
    }

    public async Task<TaskErrorResult<UserDTO>> GetUserByEmailAsync(string email, string token)
    {
        _logger.LogInformation("Start, Fetching user with Email: {Email}", email);

        if (!ValidateToken(token, out int userId))
        {
            _logger.LogWarning("Invalid or expired token.");
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorInvalidCredentials, "Invalid token.");
        }

        if (userId <= 0)
        {
            _logger.LogWarning("Invalid user ID: {UserId}", userId);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid user ID.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("Invalid email: {Email}", email);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorInvalidEmail, "Invalid email address.");
        }

        var userEntity = await _unitOfWork.UserRepository.GetUserByEmailAsync(email);
        if (userEntity == null)
        {
            _logger.LogWarning("User with Email {Email} not found.", email);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorUserNotFound, "User not found.");
        }

        var userDto = _mapper.Map<UserDTO>(userEntity);
        _logger.LogInformation("End, Fetching user with Email: {Email}", email);
        return TaskErrorResult<UserDTO>.Success(userDto);
    }

}