// File: Core/TaskManagementSystem.Application/Services/AuthService.cs
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

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;
    private readonly IJwtService _jwtService;

    public AuthService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AuthService> logger, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<TaskErrorResult<UserDTO>> RegisterAsync(CreateUserDto createUserDto)
    {
        _logger.LogInformation("Start, Registering user with email: {Email}", createUserDto.Email);

        if (string.IsNullOrWhiteSpace(createUserDto.Email) || !IsValidEmail(createUserDto.Email))
        {
            _logger.LogWarning("Invalid email: {Email}", createUserDto.Email);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorInvalidEmail, "Invalid email.");
        }

        var existingUser = await _unitOfWork.UserRepository.GetUserByEmailAsync(createUserDto.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("Email already in use: {Email}", createUserDto.Email);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorEmailAlreadyExists, "Email already in use.");
        }

        if (string.IsNullOrWhiteSpace(createUserDto.FirstName) || string.IsNullOrWhiteSpace(createUserDto.LastName))
        {
            _logger.LogWarning("First name or last name is missing: {FirstName} {LastName}", createUserDto.FirstName, createUserDto.LastName);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorInvalidName, "First name or last name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(createUserDto.Password) || !IsValidPassword(createUserDto.Password))
        {
            _logger.LogWarning("Weak password provided for email: {Email} with password {Password}", createUserDto.Email, createUserDto.Password);
            return TaskErrorResult<UserDTO>.Failure(TaskErrorType.ErrorWeakPassword, "Password is too weak. It must have at least 8 characters, including a number, an uppercase letter, and a special character.");
        }

        var hashedPassword = _unitOfWork.PasswordHasher.HashPassword(createUserDto.Password);

        var userEntity = new UserEntity
        {
            UserId = Guid.NewGuid(),
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            PasswordHash = hashedPassword,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            Role = Role.User
        };

        await _unitOfWork.UserRepository.AddUserAsync(userEntity);
        await _unitOfWork.CommitAsync();

        var userDto = _mapper.Map<UserDTO>(userEntity);
        _logger.LogInformation("End, Registering user with email: {Email}", createUserDto.Email);
        return TaskErrorResult<UserDTO>.Success(userDto);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidPassword(string password)
    {
        var regex = new System.Text.RegularExpressions.Regex(@"^(?=.*[A-Z])(?=.*\d).{8,}$");
        return regex.IsMatch(password);
    }


    public async Task<TaskErrorResult<AuthResponseDTO>> LoginAsync(LoginDTO loginDto)
    {
        _logger.LogInformation("Start, Logging in user with email: {Email}", loginDto.Email);

        var userEntity = await _unitOfWork.UserRepository.GetUserByEmailAsync(loginDto.Email);
        if (userEntity == null)
        {
            _logger.LogWarning("Email not found in user: {Email}", loginDto.Email);
            return TaskErrorResult<AuthResponseDTO>.Failure(TaskErrorType.ErrorInvalidCredentials, "Email not found in user");
        }

        var verifyPassword = _unitOfWork.PasswordHasher.VerifyPassword(loginDto.Password, userEntity.PasswordHash);
        if (!verifyPassword)
        {
            _logger.LogWarning("Invalid login attempt for email: {Email}", loginDto.Email);
            return TaskErrorResult<AuthResponseDTO>.Failure(TaskErrorType.ErrorInvalidCredentials, "Invalid credentials.");
        }

        var userDto = _mapper.Map<UserDTO>(userEntity);
        string token = _jwtService.GenerateToken(userDto);
        var authResponse = new AuthResponseDTO
        {
            Token = token,
            User = userDto
        };

        _logger.LogInformation("End, Logging in user with email: {Email}", loginDto.Email);
        return TaskErrorResult<AuthResponseDTO>.Success(authResponse);
    }
}