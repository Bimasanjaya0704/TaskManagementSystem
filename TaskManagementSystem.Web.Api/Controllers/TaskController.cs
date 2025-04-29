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

    public TaskController(ITaskService taskService, ILogger<TaskController> logger, TokenService tokenService)
    {
        _taskService = taskService;
        _logger = logger;
        _tokenService = tokenService;
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllTask()
    {
        _logger.LogInformation("Start, GetAllTasks");

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("End, GetAllTasks - Failed: Token is missing");
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var result = await _taskService.GetAllAsync();

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, GetAllTasks - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var taskResponseDto = result.Data.Select(task => new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status,
            AssignedToUserId = task.AssignedToUserId,
            ReviewedByUserId = task.ReviewedByUserId,
            ProjectId = task.ProjectId
        }).ToList();

        _logger.LogInformation("End, GetAllTasks - Success, Retrieved {Count} tasks", taskResponseDto.Count);
        return Ok(new ApiResponse<List<TaskResponseDto>>(true, "Success", taskResponseDto));
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        _logger.LogInformation("Start, GetTaskById: {TaskId}", id);

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("End, GetTaskById - Failed: Token is missing");
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var result = await _taskService.GetByIdAsync(id);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, GetTaskById - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var taskResponseDto = new TaskResponseDto
        {
            Id = result.Data.Id,
            Title = result.Data.Title,
            Description = result.Data.Description,
            DueDate = result.Data.DueDate,
            Status = result.Data.Status,
            AssignedToUserId = result.Data.AssignedToUserId,
            ReviewedByUserId = result.Data.ReviewedByUserId,
            ProjectId = result.Data.ProjectId
        };

        _logger.LogInformation("End, GetTaskById - Success: Found Task {TaskId}", id);
        return Ok(new ApiResponse<TaskResponseDto>(true, "Success", taskResponseDto));
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TaskRequestDto taskRequestDto)
    {
        _logger.LogInformation("Start, CreateTask");

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("End, CreateTask - Failed: Token is missing");
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var result = await _taskService.AddAsync(new TaskDTO()
        {
            Title = taskRequestDto.Title,
            Description = taskRequestDto.Description,
            DueDate = taskRequestDto.DueDate,
            Status = taskRequestDto.Status,
            AssignedToUserId = taskRequestDto.AssignedToUserId,
            ReviewedByUserId = taskRequestDto.ReviewedByUserId,
            ProjectId = taskRequestDto.ProjectId
        });

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, CreateTask - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var taskResponseDto = new TaskResponseDto
        {
            Id = result.Data.Id,
            Title = result.Data.Title,
            Description = result.Data.Description,
            DueDate = result.Data.DueDate,
            Status = result.Data.Status,
            AssignedToUserId = result.Data.AssignedToUserId,
            ReviewedByUserId = result.Data.ReviewedByUserId,
            ProjectId = result.Data.ProjectId

        };

        _logger.LogInformation("End, CreateTask - Success: Created Task {TaskId}", taskResponseDto.Id);
        return Ok(new ApiResponse<TaskResponseDto>(true, "Success", taskResponseDto));
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskRequestDto taskRequestDto)
    {
        _logger.LogInformation("Start, UpdateTask: {TaskId}", id);

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("End, UpdateTask - Failed: Token is missing.");
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        if (taskRequestDto == null)
        {
            _logger.LogWarning("End, UpdateTask - Failed: Invalid request data.");
            return BadRequest(new ApiResponse<string>(false, "Invalid request data.", null));
        }

        var taskDto = new TaskDTO()
        {
            Id = id,
            Title = taskRequestDto.Title,
            Description = taskRequestDto.Description,
            DueDate = taskRequestDto.DueDate,
            Status = taskRequestDto.Status,
            AssignedToUserId = taskRequestDto.AssignedToUserId,
            ReviewedByUserId = taskRequestDto.ReviewedByUserId,
            ProjectId = taskRequestDto.ProjectId

        };

        var result = await _taskService.UpdateAsync(id, taskDto);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, UpdateTask - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var taskResponseDto = new TaskResponseDto
        {
            Id = result.Data.Id,
            Title = result.Data.Title,
            Description = result.Data.Description,
            DueDate = result.Data.DueDate,
            Status = result.Data.Status,
            AssignedToUserId = result.Data.AssignedToUserId,
            ReviewedByUserId = result.Data.ReviewedByUserId,
            ProjectId = result.Data.ProjectId

        };

        _logger.LogInformation("End, UpdateTask - Success: Updated Task {TaskId}", id);
        return Ok(new ApiResponse<TaskResponseDto>(true, "Task updated successfully.", taskResponseDto));
    }

    [Authorize(Roles = "User,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        _logger.LogInformation("Start, DeleteTask: {TaskId}", id);

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("End, DeleteTask - Failed: Token is missing.");
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var result = await _taskService.DeleteAsync(id);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, DeleteTask - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var deletedTask = new TaskResponseDto
        {
            Id = result.Data.Id,
            Title = result.Data.Title,
            Description = result.Data.Description,
            DueDate = result.Data.DueDate,
            Status = result.Data.Status,
            AssignedToUserId = result.Data.AssignedToUserId,
            ReviewedByUserId = result.Data.ReviewedByUserId,
            ProjectId = result.Data.ProjectId
        };

        _logger.LogInformation("End, DeleteTask - Success: Deleted Task {TaskId}", id);
        return Ok(new ApiResponse<TaskResponseDto>(true, "Task deleted successfully.", deletedTask));
    }
}