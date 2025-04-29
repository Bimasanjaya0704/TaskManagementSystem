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

    public async Task<TaskErrorResult<TaskDTO>> GetByIdAsync(int id)
    {
        _logger.LogInformation("Start, Fetching task with ID: {TaskId}", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid task ID: {TaskId}", id);
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid task ID.");
        }

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

    public async Task<TaskErrorResult<IEnumerable<TaskDTO>>> GetAllAsync()
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

    public async Task<TaskErrorResult<TaskDTO>> AddAsync(TaskDTO taskDto)
    {
        _logger.LogInformation("Start, Adding new task");

        if (taskDto == null)
        {
            _logger.LogWarning("Task data is null.");
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorTaskNotFound, "Task data cannot be null.");
        }
        
        if (taskDto.ProjectId <= 0)
        {
            _logger.LogWarning("Invalid ProjectId: {ProjectId}", taskDto.ProjectId);
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid Project ID.");
        }


        var taskEntity = _mapper.Map<TaskEntity>(taskDto);
        await _unitOfWork.TaskRepository.AddTaskAsync(taskEntity);
        await _unitOfWork.CommitAsync();

        var createdTaskDto = _mapper.Map<TaskDTO>(taskEntity);
        _logger.LogInformation("End, Adding new task");
        return TaskErrorResult<TaskDTO>.Success(createdTaskDto);
    }

    public async Task<TaskErrorResult<TaskDTO>> UpdateAsync(int id, TaskDTO taskDto)
    {
        _logger.LogInformation("Start, Updating task with ID: {TaskId}", id);

        if (id <= 0 || taskDto == null)
        {
            _logger.LogWarning("Invalid task data: {TaskId}", id);
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid task data.");
        }
        
        if (taskDto.Id != 0 && taskDto.Id != id)
        {
            _logger.LogWarning("Mismatched task ID: {TaskId} (Route ID: {RouteId})", taskDto.Id, id);
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorInvalidId, "Task ID mismatch.");
        }
        
        if (taskDto.ProjectId <= 0)
        {
            _logger.LogWarning("Invalid ProjectId: {ProjectId}", taskDto.ProjectId);
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid Project ID.");
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


    public async Task<TaskErrorResult<TaskDTO>> DeleteAsync(int id)
    {
        _logger.LogInformation("Start, Deleting task with ID: {TaskId}", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid task ID: {TaskId}", id);
            return TaskErrorResult<TaskDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid task ID.");
        }

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
}
