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
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TaskController> _logger;
    private readonly TokenService _tokenService;
    private readonly IMapper _mapper;

    public TaskController(ITaskService taskService, ILogger<TaskController> logger, TokenService tokenService, IMapper mapper)
    {
        _taskService = taskService;
        _logger = logger;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    private string GetTokenFromRequest()
    {
        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Token is missing");
            return null;
        }
        return token;
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllTasks()
    {
        _logger.LogInformation("Start, GetAllTasks");

        var token = GetTokenFromRequest();
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var result = await _taskService.GetAllTasksAsync();

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, GetAllTasks - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var taskResponseDto = _mapper.Map<List<TaskResponseDto>>(result.Data);

        _logger.LogInformation("End, GetAllTasks - Success, Retrieved {Count} tasks", taskResponseDto.Count);
        return Ok(new ApiResponse<List<TaskResponseDto>>(true, "Success", taskResponseDto));
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        _logger.LogInformation("Start, GetTaskById: {TaskId}", id);

        var token = GetTokenFromRequest();
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var result = await _taskService.GetTaskByIdAsync(id);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, GetTaskById - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var taskResponseDto = _mapper.Map<TaskResponseDto>(result.Data);

        _logger.LogInformation("End, GetTaskById - Success: Found Task {TaskId}", id);
        return Ok(new ApiResponse<TaskResponseDto>(true, "Success", taskResponseDto));
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TaskRequestDto taskRequestDto)
    {
        _logger.LogInformation("Start, CreateTask");

        var token = GetTokenFromRequest();
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        if (taskRequestDto == null)
        {
            return BadRequest(new ApiResponse<string>(false, "Invalid request data.", null));
        }

        var taskDto = _mapper.Map<CreateTaskDto>(taskRequestDto);
        var result = await _taskService.CreateTaskAsync(taskDto);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, CreateTask - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var taskResponseDto = _mapper.Map<TaskResponseDto>(result.Data);

        _logger.LogInformation("End, CreateTask - Success: Created Task {TaskId}", taskResponseDto.Id);
        return Ok(new ApiResponse<TaskResponseDto>(true, "Task created successfully.", taskResponseDto));
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskRequestDto taskRequestDto)
    {
        _logger.LogInformation("Start, UpdateTask: {TaskId}", id);

        var token = GetTokenFromRequest();
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        if (taskRequestDto == null)
        {
            return BadRequest(new ApiResponse<string>(false, "Invalid request data.", null));
        }

        var taskDto = _mapper.Map<TaskDTO>(taskRequestDto);
        var result = await _taskService.UpdateTaskAsync(id, taskDto);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, UpdateTask - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var taskResponseDto = _mapper.Map<TaskResponseDto>(result.Data);

        _logger.LogInformation("End, UpdateTask - Success: Updated Task {TaskId}", id);
        return Ok(new ApiResponse<TaskResponseDto>(true, "Task updated successfully.", taskResponseDto));
    }

    [Authorize(Roles = "User,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        _logger.LogInformation("Start, DeleteTask: {TaskId}", id);

        var token = GetTokenFromRequest();
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var result = await _taskService.DeleteTaskAsync(id);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, DeleteTask - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        _logger.LogInformation("End, DeleteTask - Success: Deleted Task {TaskId}", id);
        return Ok(new ApiResponse<string>(true, "Task deleted successfully.", null));
    }
}
