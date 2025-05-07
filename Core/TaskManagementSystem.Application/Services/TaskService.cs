using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Enum;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Result;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;

namespace TaskManagementSystem.Application.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TaskService> _logger;

    public TaskService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TaskService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TaskErrorResult<TaskDTO>> GetTaskByIdAsync(Guid id)
    {
        _logger.LogInformation("Start, Fetching task with ID: {TaskId}", id);

        var taskEntity = await _unitOfWork.TaskRepository.GetTaskByIdAsync(id);
        if (taskEntity == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found.", id);
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorTaskNotFound, "Task not found.");
        }

        var taskDto = _mapper.Map<TaskDTO>(taskEntity);
        _logger.LogInformation("End, Fetching task with ID: {TaskId}", id);
        return TaskErrorResult<TaskDTO>.Success(taskDto);
    }

    public async Task<TaskErrorResult<IEnumerable<TaskDTO>>> GetAllTasksAsync()
    {
        _logger.LogInformation("Start, Fetching all tasks");
        var taskEntities = await _unitOfWork.TaskRepository.GetAllTaskAsync();

        if (taskEntities == null || !taskEntities.Any())
        {
            _logger.LogWarning("No tasks found.");
            return TaskErrorResult<IEnumerable<TaskDTO>>.Failure(TaskErrorType.ErrorTaskNotFound, "No tasks found.");
        }

        var taskDtos = _mapper.Map<IEnumerable<TaskDTO>>(taskEntities);
        _logger.LogInformation("End, Fetching all tasks");
        return TaskErrorResult<IEnumerable<TaskDTO>>.Success(taskDtos);
    }

    public async Task<TaskErrorResult<TaskDTO>> CreateTaskAsync(CreateTaskDto taskDto)
    {
        _logger.LogInformation("Start, Adding new task");

        if (taskDto == null)
        {
            _logger.LogWarning("Task data is null.");
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorTaskNotFound, "Task data cannot be null.");
        }

        var taskEntity = new TaskEntity()
        {
            TaskId = new Guid(),
            Title = taskDto.Title,
            Description = taskDto.Description,
            Status = taskDto.Status,
            DueDate = taskDto.DueDate,
            ProjectId = taskDto.ProjectId,
            AssignedToUserId = taskDto.AssignedToUserId,
            ReviewedToUserId = taskDto.ReviewedToUserId,
        };
        
        await _unitOfWork.TaskRepository.AddTaskAsync(taskEntity);
        await _unitOfWork.CommitAsync();

        var createdTaskDto = _mapper.Map<TaskDTO>(taskEntity);
        _logger.LogInformation("End, Adding new task");
        return TaskErrorResult<TaskDTO>.Success(createdTaskDto);
    }

    public async Task<TaskErrorResult<TaskDTO>> UpdateTaskAsync(Guid id, TaskDTO taskDto)
    {
        _logger.LogInformation("Start, Updating task with ID: {TaskId}", id);

        if (taskDto == null)
        {
            _logger.LogWarning("Invalid task data: {TaskId}", id);
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid task data.");
        }
        
        if (taskDto.TaskId != id)
        {
            _logger.LogWarning("Mismatched task ID: {TaskId} (Route ID: {RouteId})", taskDto.TaskId, id);
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorInvalidId, "Task ID mismatch.");
        }
      
        var existingTask = await _unitOfWork.TaskRepository.GetTaskByIdAsync(id);
        if (existingTask == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found.", id);
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorTaskNotFound, "Task not found.");
        }

        _mapper.Map(taskDto, existingTask);
        await _unitOfWork.TaskRepository.UpdateTaskAsync(id, existingTask);
        await _unitOfWork.CommitAsync();

        var updatedTaskDto = _mapper.Map<TaskDTO>(existingTask);
        _logger.LogInformation("End, Updating task with ID: {TaskId}", id);
        return TaskErrorResult<TaskDTO>.Success(updatedTaskDto);
    }


    public async Task<TaskErrorResult<TaskDTO>> DeleteTaskAsync(Guid id)
    {
        _logger.LogInformation("Start, Deleting task with ID: {TaskId}", id);

        var existingTask = await _unitOfWork.TaskRepository.GetTaskByIdAsync(id);
        if (existingTask == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found.", id);
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorTaskNotFound, "Task not found.");
        }

        var deleted = await _unitOfWork.TaskRepository.DeleteTaskAsync(id);
        if (!deleted)
        {
            _logger.LogError("Failed to delete task with ID: {TaskId}", id);
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorDeleteTask, "Failed to delete task.");
        }

        await _unitOfWork.CommitAsync();
        _logger.LogInformation("End, Deleting task with ID: {TaskId}", id);
        return TaskErrorResult<TaskDTO>.Success();
    }

    public async Task<TaskErrorResult<TaskDTO>> GetTasksByProjectIdAsync(Guid projectId)
    {
        _logger.LogInformation("Fetching tasks by Project ID: {ProjectId}", projectId);

        if (!await _unitOfWork.ProjectRepository.ExistsAsync(projectId))
        {
            _logger.LogWarning("Project with ID {ProjectId} not found.", projectId);
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorProjectNotFound, "Project not found.");
        }

        var tasks = await _unitOfWork.TaskRepository.GetByProjectIdAsync(projectId);
        if (tasks == null || !tasks.Any())
        {
            _logger.LogWarning("No tasks found for Project ID: {ProjectId}", projectId);
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorTaskNotFound, "No tasks found.");
        }

        var taskDtos = _mapper.Map<TaskDTO>(tasks);
        _logger.LogInformation("Successfully fetched tasks for Project ID: {ProjectId}", projectId);
        return TaskErrorResult<TaskDTO>.Success(taskDtos);
    }
    
    public async Task<TaskErrorResult<IEnumerable<TaskDTO>>> GetTasksAssignedToUserAsync(Guid userId)
    {
        _logger.LogInformation("Fetching tasks assigned to user with ID: {UserId}", userId);

        if (!await _unitOfWork.UserRepository.ExistsAsync(userId))
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            return TaskErrorResult<IEnumerable<TaskDTO>>.Failure(TaskErrorType.ErrorUserNotFound, "User not found.");
        }

        var tasks = await _unitOfWork.TaskRepository.GetAssignedToUserAsync(userId);
        var taskDtos = await Task.WhenAll(tasks.Select(MapToDto));

        _logger.LogInformation("Fetched {Count} assigned tasks for user ID: {UserId}", taskDtos.Length, userId);
        return TaskErrorResult<IEnumerable<TaskDTO>>.Success(taskDtos);
    }
    
    public async Task<TaskErrorResult<TaskDTO>> GetTasksReviewedToUserAsync(Guid userId)
    {
        _logger.LogInformation("Fetching tasks reviewed by user with ID: {UserId}", userId);

        if (!await _unitOfWork.UserRepository.ExistsAsync(userId))
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorUserNotFound, "User not found.");
        }

        var tasks = await _unitOfWork.TaskRepository.GetReviewedToUserAsync(userId);
        var taskDtos = _mapper.Map<TaskDTO>(tasks);

        _logger.LogInformation("Fetched reviewed tasks for user ID: {UserId}", userId);
        return TaskErrorResult<TaskDTO>.Success(taskDtos);
    }
    
    public async Task<bool> TaskExistsAsync(Guid taskId)
    {
        _logger.LogInformation("Checking if task exists with ID: {TaskId}", taskId);
        return await _unitOfWork.TaskRepository.ExistsAsync(taskId);
    }

    public async Task AssignTaskToUserAsync(Guid taskId, Guid userId)
    {
        _logger.LogInformation("Assigning task {TaskId} to user {UserId}", taskId, userId);

        var task = await _unitOfWork.TaskRepository.GetTaskByIdAsync(taskId);
        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found.", taskId);
            throw new KeyNotFoundException($"Task with ID {taskId} not found");
        }

        if (!await _unitOfWork.UserRepository.ExistsAsync(userId))
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        if (!await _unitOfWork.ProjectRepository.IsUserMemberAsync(task.ProjectId, userId))
        {
            _logger.LogWarning("User {UserId} is not a member of project {ProjectId}", userId, task.ProjectId);
            throw new InvalidOperationException($"User is not a member of the project");
        }

        task.AssignedToUserId = userId;
        await _unitOfWork.TaskRepository.UpdateTaskAsync(task.TaskId, task);
        _logger.LogInformation("Task {TaskId} assigned to user {UserId}", taskId, userId);
    }

    public async Task ReviewTaskToUserAsync(Guid taskId, Guid userId)
    {
        _logger.LogInformation("Assigning task reviewer. Task: {TaskId}, Reviewer: {UserId}", taskId, userId);

        var task = await _unitOfWork.TaskRepository.GetTaskByIdAsync(taskId);
        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found.", taskId);
            throw new KeyNotFoundException($"Task with ID {taskId} not found");
        }

        if (!await _unitOfWork.UserRepository.ExistsAsync(userId))
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        if (!await _unitOfWork.ProjectRepository.IsUserMemberAsync(task.ProjectId, userId))
        {
            _logger.LogWarning("User {UserId} is not a member of project {ProjectId}", userId, task.ProjectId);
            throw new InvalidOperationException($"User is not a member of the project");
        }

        task.ReviewedToUserId = userId;
        await _unitOfWork.TaskRepository.UpdateTaskAsync(task.TaskId, task);
        _logger.LogInformation("Task {TaskId} set to be reviewed by user {UserId}", taskId, userId);
    }

    public async Task UpdateTaskStatusAsync(Guid taskId, Guid status)
    {
        _logger.LogInformation("Updating status of task with ID: {TaskId}", taskId);

        var task = await _unitOfWork.TaskRepository.GetTaskByIdAsync(taskId);
        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found.", taskId);
            throw new KeyNotFoundException($"Task with ID {taskId} not found");
        }

        // TODO: Assign new status here

        await _unitOfWork.TaskRepository.UpdateTaskAsync(task.TaskId, task);
        _logger.LogInformation("Task status updated for task ID: {TaskId}", taskId);
    }


    private async Task<TaskDTO> MapToDto(TaskEntity task)
    {
        var assignedTo = await GetUserDtoByIdAsync(task.AssignedToUserId);
        var reviewedTo = await GetUserDtoByIdAsync(task.ReviewedToUserId);

        return new TaskDTO
        {
            TaskId = task.TaskId,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            DueDate = task.DueDate,
            ProjectId = task.ProjectId,
            AssignedToUserId = assignedTo.UserId,
            ReviewedByUserId = reviewedTo.UserId
        };
    }

    private async Task<UserDTO> GetUserDtoByIdAsync(Guid? userId)
    {
        if (!userId.HasValue)
        {
            _logger.LogInformation("User ID is null, skipping user fetch.");
            return null;
        }

        var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId.Value);
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId.Value);
            return null;
        }

        _logger.LogInformation("Fetched user with ID {UserId}", user.UserId);

        return new UserDTO
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };
    }
}
