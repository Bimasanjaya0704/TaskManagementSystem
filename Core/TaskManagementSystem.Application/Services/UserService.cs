using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Enum;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Result;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enum;
using TaskManagementSystem.Domain.Interfaces;

namespace TaskManagementSystem.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(IUnitOfWork unitOfWork, IUserContextService userContextService, IMapper mapper, ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TaskErrorResult<UserDTO>> GetUserByIdAsync(Guid id)
    {
        _logger.LogInformation("Start, Fetching user with ID: {UserId}", id);

        var currentRole = _userContextService.GetCurrentUserRole();
        var currentUserId = _userContextService.GetCurrentUserId();
        
        if (currentRole == Role.User && id != currentUserId)
        {
            var friends = await _unitOfWork.UserRepository.GetFriendsAsync(currentUserId);
            if (friends == null || !friends.Any(friend => friend.UserId == id))
            {
                _logger.LogWarning("User with ID {UserId} is not allowed to access user {RequestedUserId}.", currentUserId, id);
                return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorUnauthorized, "You are not authorized to view this user.");
            }
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

    public async Task<TaskErrorResult<IEnumerable<UserDTO>>> GetAllUsersAsync()
    {
        _logger.LogInformation("Start, Fetching all users");

        var role = _userContextService.GetCurrentUserRole();
        var currentUserId = _userContextService.GetCurrentUserId();

        if (role == Role.Unknown)
        {
            _logger.LogWarning("Role Unknown.");
            return TaskErrorResult<IEnumerable<UserDTO>>.Failure(TaskErrorType.ErrorUnauthorized, "Role Unknown.");
        }
        
        if (role == Role.User)
        {
            var friends = await _unitOfWork.UserRepository.GetFriendsAsync(currentUserId);
            if (friends == null || !friends.Any())
            {
                _logger.LogWarning("No friends found.");
                return TaskErrorResult<IEnumerable<UserDTO>>.Failure(TaskErrorType.ErrorUserNotFound, "No friends found.");
            }

            var userDtos = _mapper.Map<IEnumerable<UserDTO>>(friends);
            _logger.LogInformation("End, Fetching user friends");
            return TaskErrorResult<IEnumerable<UserDTO>>.Success(userDtos);
        }
        else
        {
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
    }

    public async Task<TaskErrorResult<UserDTO>> CreateUserAsync(CreateUserDtoAdmin createUserDto)
    {
        _logger.LogInformation("Start, Creating User");

        var role = _userContextService.GetCurrentUserRole();
        if (role  == Role.User || role == Role.Unknown)
        {
            _logger.LogWarning("User with Role {Role} is not authorized to create new users.", role);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorUnauthorized, "You are not authorized to create new users.");
        }

        var emailExist = await _unitOfWork.UserRepository.ExistsByEmailAsync(createUserDto.Email);
        if (emailExist)
        {
            _logger.LogWarning("Email is already in use.");
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorEmailAlreadyExists, "Email already in use.");
        }

        var usernameExist = await _unitOfWork.UserRepository.ExistsByUsernameAsync(createUserDto.Username);
        if (usernameExist)
        {
            _logger.LogWarning("Username is already in use.");
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorUsernameIsAlreadyExist, "Username already in use.");
        }
        var hashedPassword = _unitOfWork.PasswordHasher.HashPassword(createUserDto.Password);


        var userEntity = new UserEntity
        {
            UserId = Guid.NewGuid(),
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            PasswordHash = hashedPassword,  
            Role = createUserDto.Role
        };

        await _unitOfWork.UserRepository.AddUserAsync(userEntity);
        await _unitOfWork.CommitAsync();

        var userDto = _mapper.Map<UserDTO>(userEntity);

        _logger.LogInformation("End, User Created");

        return TaskErrorResult<UserDTO>.Success(userDto);
    }


    public async Task<TaskErrorResult<UserDTO>> UpdateUserAsync(Guid id, UpdateUserDto userDto)
    {
        _logger.LogInformation("Start, Updating user with ID: {UserId}", id);

        var currentRole = _userContextService.GetCurrentUserRole();
        var currentUserId = _userContextService.GetCurrentUserId();
        
        if (currentRole == Role.User && id != currentUserId)
        {
            _logger.LogWarning("User with ID {UserId} is not authorized to update user {TargetUserId}.", currentUserId, id);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorUnauthorized, "You are not authorized to update this user.");
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

    public async Task<TaskErrorResult<UserDTO>> DeleteUserAsync(Guid id)
    {
        _logger.LogInformation("Start, Deleting user with ID: {UserId}", id);
        
        var currentRole = _userContextService.GetCurrentUserRole();
        var currentUserId = _userContextService.GetCurrentUserId();
        if (currentRole == Role.User && id != currentUserId)
        {
            _logger.LogWarning("User with ID {UserId} is not authorized to delete user {TargetUserId}.", currentUserId, id);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorUnauthorized, "You are not authorized to delete this user.");
        }
        
        var existingUser = await _unitOfWork.UserRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", id);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorUserNotFound, "User not found.");
        }
        
        if (existingUser.Role == Role.Admin)
        {
            var adminCount = await _unitOfWork.UserRepository.CountUsersByRoleAsync(Role.Admin);
            if (adminCount <= 1)
            {
                _logger.LogWarning("Cannot delete the last admin user.");
                return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorDeleteUser, "Cannot delete the last admin user.");
            }
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

    public async Task<TaskErrorResult<UserDTO>> GetUserByEmailAsync(string email)
    {
        _logger.LogInformation("Start, Get user with Email: {Email}", email);
        
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

    public async Task<TaskErrorResult<UserDTO>> GetUserByUsernameAsync(string username)
    {
        _logger.LogInformation("Start, Get user with Username: {Username}", username);
        
        if (string.IsNullOrWhiteSpace(username))
        {
            _logger.LogWarning("Invalid username: {Username}", username);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorInvalidUsername, "Invalid email address.");
        }

        var userEntity = await _unitOfWork.UserRepository.GetByUsernameAsync(username);
        if (userEntity == null)
        {
            _logger.LogWarning("User with Username {Username} not found.", username);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorUserNotFound, "User not found.");
        }

        var userDto = _mapper.Map<UserDTO>(userEntity);
        _logger.LogInformation("End, Get user with Username: {Username}", username);

        return TaskErrorResult<UserDTO>.Success(userDto);
    }
}