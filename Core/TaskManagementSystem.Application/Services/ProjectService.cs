using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Enum;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Result;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;

namespace TaskManagementSystem.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProjectService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TaskErrorResult<ProjectDTO>> GetProjectByIdAsync(int id)
    {
        _logger.LogInformation("Start, Fetching project with ID: {ProjectId}", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid project ID: {ProjectId}", id);
            return TaskErrorResult<ProjectDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid project ID.");
        }

        var projectEntity = await _unitOfWork.ProjectRepository.GetProjectByIdAsync(id);
        if (projectEntity == null)
        {
            _logger.LogWarning("Project with ID {TaskId} not found.", id);
            return TaskErrorResult<ProjectDTO>.Failure(TaskErrorType.ErrorProjectNotFound, "Project not found.");
        }

        var projectDto = _mapper.Map<ProjectDTO>(projectEntity);
        _logger.LogInformation("End, Fetching project with ID: {TaskId}", id);
        return TaskErrorResult<ProjectDTO>.Success(projectDto);
    }

    public async Task<TaskErrorResult<IEnumerable<ProjectDTO>>> GetAllProjectAsync()
    {
        _logger.LogInformation("Start, Fetching all projects");
        var projectEntities = await _unitOfWork.ProjectRepository.GetAllProjectsAsync();

        if (projectEntities == null || !projectEntities.Any())
        {
            _logger.LogWarning("No projects found.");
            return TaskErrorResult<IEnumerable<ProjectDTO>>.Failure(TaskErrorType.ErrorProjectNotFound, "No projects found.");
        }

        var projectDtos = _mapper.Map<IEnumerable<ProjectDTO>>(projectEntities);
        _logger.LogInformation("End, Fetching all projects");
        return TaskErrorResult<IEnumerable<ProjectDTO>>.Success(projectDtos);
    }

    public async Task<TaskErrorResult<ProjectDTO>> AddProjectAsync(ProjectDTO projectDto)
    {
        _logger.LogInformation("Start, Adding a new project to the database: {ProjectName}", projectDto.Name);

        if (projectDto == null)
        {
            _logger.LogWarning("Project data is null.");
            return TaskErrorResult<ProjectDTO>.Failure(TaskErrorType.ErrorProjectNotFound, "Project data cannot be null.");
        }

        if (projectDto.CreatedByUserId <= 0)
        {
            _logger.LogWarning("Invalid Id.");
            return TaskErrorResult<ProjectDTO>.Failure(TaskErrorType.ErrorInvalidId, "Id invalid, must be longer than 0.");
        }

        var projectEntity = _mapper.Map<ProjectEntity>(projectDto);
        await _unitOfWork.ProjectRepository.AddProjectAsync(projectEntity);
        await _unitOfWork.CommitAsync();

        var addedProjectDto = _mapper.Map<ProjectDTO>(projectEntity);
        _logger.LogInformation("End, Project added successfully with ID {ProjectId}.", addedProjectDto.Id);
        return TaskErrorResult<ProjectDTO>.Success(addedProjectDto);
    }

    public async Task<TaskErrorResult<ProjectDTO>> UpdateProjectAsync(int id, ProjectDTO projectDto)
    {
        _logger.LogInformation("Start, Updating project with ID: {ProjectId}", id);

        if (id <= 0 || projectDto == null)
        {
            _logger.LogWarning("Invalid project data: {ProjectId}", id);
            return TaskErrorResult<ProjectDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid project data.");
        }

        if (projectDto.Id != 0 && projectDto.Id != id)
        {
            _logger.LogWarning("Invalid project ID: {ProjectId}", projectDto.Id);
            return TaskErrorResult<ProjectDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid project ID.");
        }

        var existingProject = await _unitOfWork.ProjectRepository.GetProjectByIdAsync(id);
        if (existingProject == null)
        {
            _logger.LogWarning("Project with ID {ProjectId} not found.", id);
            return TaskErrorResult<ProjectDTO>.Failure(TaskErrorType.ErrorProjectNotFound, "Project not found.");
        }

        existingProject.UpdateProjectDate();
        _mapper.Map(projectDto, existingProject);
        await _unitOfWork.ProjectRepository.UpdateProjectAsync(id, existingProject);
        await _unitOfWork.CommitAsync();

        var updatedProjectDto = _mapper.Map<ProjectDTO>(existingProject);
        _logger.LogInformation("End, Updating project with ID: {ProjectId}", id);
        return TaskErrorResult<ProjectDTO>.Success(updatedProjectDto);
    }

    public async Task<TaskErrorResult<ProjectDTO>> DeleteProjectAsync(int id)
    {
        _logger.LogInformation("Start, Deleting project with ID: {ProjectId}", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid project ID: {ProjectId}", id);
            return TaskErrorResult<ProjectDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid project ID.");
        }

        var existingProject = await _unitOfWork.ProjectRepository.GetProjectByIdAsync(id);
        if (existingProject == null)
        {
            _logger.LogWarning("Project with ID {ProjectId} not found.", id);
            return TaskErrorResult<ProjectDTO>.Failure(TaskErrorType.ErrorProjectNotFound, "Project not found.");
        }

        var deletedProjectDto = _mapper.Map<ProjectDTO>(existingProject);

        var deleted = await _unitOfWork.ProjectRepository.DeleteProjectAsync(id);
        if (!deleted)
        {
            _logger.LogWarning("Failed to delete project with ID {ProjectId}.", id);
            return TaskErrorResult<ProjectDTO>.Failure(TaskErrorType.ErrorDeleteProject, "Failed to delete project.");
        }

        await _unitOfWork.CommitAsync();
        _logger.LogInformation("End, Deleting project with ID: {ProjectId}", id);
        return TaskErrorResult<ProjectDTO>.Success(deletedProjectDto);
    }
}