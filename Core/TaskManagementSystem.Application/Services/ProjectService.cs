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

    public async Task<TaskErrorResult<ProjectDTO>> GetProjectByIdAsync(Guid id)
    {
        _logger.LogInformation("Start, Fetching project with ID: {ProjectId}", id);

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
    
    public async Task<TaskErrorResult<IEnumerable<ProjectDTO>>> GetUserProjectsAsync(Guid userId)
    {
        _logger.LogInformation("Start,GetUserProjects project with ID: {UserId}", userId);
        
        var projectEntity = await _unitOfWork.ProjectRepository.GetByUserIdAsync(userId);
        if (projectEntity == null)
        {
            _logger.LogWarning("Project with ID {userId} not found.", userId);
            return TaskErrorResult<IEnumerable<ProjectDTO>>.Failure(TaskErrorType.ErrorProjectNotFound, "Project not found.");
        }
        
        _logger.LogInformation("End,GetUserProjects project with ID: {UserId}", userId);
        var projectDtos = await Task.WhenAll(projectEntity.Select(MapToDto));
        
        return TaskErrorResult<IEnumerable<ProjectDTO>>.Success(projectDtos);
    }

    public async Task<TaskErrorResult<ProjectDTO>> CreateProjectAsync(CreateProjectDto createProjectDto)
    {
        _logger.LogInformation("Start, Adding a new project to the database: {ProjectName}", createProjectDto.Name);

        var user = await _unitOfWork.UserRepository.GetUserByIdAsync(createProjectDto.CreatorUserId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {createProjectDto.CreatorUserId} not found");

        var project = new ProjectEntity()
        {
            Name = createProjectDto.Name,
            Description = createProjectDto.Description,
            CreatorUserId = createProjectDto.CreatorUserId
        };

        var addedProject = await _unitOfWork.ProjectRepository.AddProjectAsync(project);

        // Add creator as an owner member
        var projectMember = new ProjectMemberEntity()
        {
            ProjectId = addedProject.Id,
            UserId = createProjectDto.CreatorUserId,
            Role = ProjectRole.Owner
        };

        await _unitOfWork.ProjectMemberRepository.AddAsync(projectMember);

        var addedProjectDto =   await MapToDto(addedProject);
        _logger.LogInformation("End, Project added successfully with ID {ProjectId}.", addedProjectDto.ProjectId);
        return TaskErrorResult<ProjectDTO>.Success(addedProjectDto);
    }

    public async Task<TaskErrorResult<ProjectDTO>> UpdateProjectAsync(Guid id, ProjectDTO projectDto)
    {
        _logger.LogInformation("Start, Updating project with ID: {ProjectId}", id);

        if ( projectDto == null)
        {
            _logger.LogWarning("Invalid project data: {ProjectId}", id);
            return TaskErrorResult<ProjectDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid project data.");
        }

        if ( projectDto.ProjectId != id)
        {
            _logger.LogWarning("Invalid project ID: {ProjectId}", projectDto.ProjectId);
            return TaskErrorResult<ProjectDTO>.Failure(TaskErrorType.ErrorInvalidId, "Invalid project ID.");
        }

        var existingProject = await _unitOfWork.ProjectRepository.GetProjectByIdAsync(id);
        if (existingProject == null)
        {
            _logger.LogWarning("Project with ID {ProjectId} not found.", id);
            return TaskErrorResult<ProjectDTO>.Failure(TaskErrorType.ErrorProjectNotFound, "Project not found.");
        }

        _mapper.Map(projectDto, existingProject);
        await _unitOfWork.ProjectRepository.UpdateProjectAsync(id, existingProject);
        await _unitOfWork.CommitAsync();

        var updatedProjectDto = _mapper.Map<ProjectDTO>(existingProject);
        _logger.LogInformation("End, Updating project with ID: {ProjectId}", id);
        return TaskErrorResult<ProjectDTO>.Success(updatedProjectDto);
    }

    public async Task<TaskErrorResult<ProjectDTO>> DeleteProjectAsync(Guid id)
    {
        _logger.LogInformation("Start, Deleting project with ID: {ProjectId}", id);

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
    
    public async Task<bool> ProjectExistsAsync(Guid projectId)
    {
        _logger.LogInformation("Start, Checking Exist project with ID: {ProjectId}", projectId);
        return await  _unitOfWork.ProjectRepository.ExistsAsync(projectId);
    }

    public async Task<bool> IsUserProjectMemberAsync(Guid projectId, Guid userId)
    {
        _logger.LogInformation("Start, Checking IsUserProjectMember project with ID: {ProjectId}", projectId);

        return await  _unitOfWork.ProjectRepository.IsUserMemberAsync(projectId, userId);
    }

    public async Task<TaskErrorResult<IEnumerable<ProjectMemberDto>>> GetProjectMembersAsync(Guid projectId)
    {
        _logger.LogInformation("Start, Fetching members for project with ID: {ProjectId}", projectId);

        var projectExists = await _unitOfWork.ProjectRepository.ExistsAsync(projectId);
        if (!projectExists)
        {
            _logger.LogWarning("Project with ID {ProjectId} not found.", projectId);
            return TaskErrorResult<IEnumerable<ProjectMemberDto>>.Failure(TaskErrorType.ErrorProjectNotFound, $"Project with ID {projectId} not found.");
        }

        var members = await _unitOfWork.ProjectMemberRepository.GetByProjectIdAsync(projectId);
    
        if (members == null || !members.Any())
        {
            _logger.LogWarning("No members found for project with ID {ProjectId}.", projectId);
            return TaskErrorResult<IEnumerable<ProjectMemberDto>>.Failure(TaskErrorType.ErrorNoMembersFound, "No members found for the project.");
        }

        var memberDtos = members.Select(MapProjectMemberToDto);
        _logger.LogInformation("End, Successfully fetched members for project with ID: {ProjectId}", projectId);

        return TaskErrorResult<IEnumerable<ProjectMemberDto>>.Success(memberDtos);
    }

   public async Task<TaskErrorResult<ProjectMemberDto>> InviteUserToProjectAsync(
    InviteUserToProjectDto inviteDto, Guid inviterUserId)
    {
        _logger.LogInformation("Start, inviting user {Username} to project {ProjectId}", inviteDto.Username, inviteDto.ProjectId);

        if (!await _unitOfWork.ProjectRepository.ExistsAsync(inviteDto.ProjectId))
        {
            _logger.LogWarning("Project {ProjectId} not found", inviteDto.ProjectId);
            return TaskErrorResult<ProjectMemberDto>.Failure(TaskErrorType.ErrorProjectNotFound, $"Project with ID {inviteDto.ProjectId} not found");
        }

        if (!await _unitOfWork.ProjectRepository.IsUserMemberAsync(inviteDto.ProjectId, inviterUserId))
        {
            _logger.LogWarning("User {InviterId} is not a member of project {ProjectId}", inviterUserId, inviteDto.ProjectId);
            return TaskErrorResult<ProjectMemberDto>.Failure(TaskErrorType.ErrorUnauthorizedMember, "You are not a member of this project");
        }

        var user = await _unitOfWork.UserRepository.GetByUsernameAsync(inviteDto.Username);
        if (user == null)
        {
            _logger.LogWarning("User {Username} not found", inviteDto.Username);
            return TaskErrorResult<ProjectMemberDto>.Failure(TaskErrorType.ErrorUserNotFound, $"User with username {inviteDto.Username} not found");
        }

        if (await _unitOfWork.ProjectMemberRepository.ExistsByProjectAndUserIdAsync(inviteDto.ProjectId, user.UserId))
        {
            _logger.LogWarning("User {Username} is already a member of project {ProjectId}", inviteDto.Username, inviteDto.ProjectId);
            return TaskErrorResult<ProjectMemberDto>.Failure(TaskErrorType.ErrorAlreadyMember, $"User {inviteDto.Username} is already a member of this project");
        }

        var role = inviteDto.ProjectRole;

        var projectMember = new ProjectMemberEntity
        {
            ProjectId = inviteDto.ProjectId,
            UserId = user.UserId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };

        var addedMember = await _unitOfWork.ProjectMemberRepository.AddAsync(projectMember);
        await _unitOfWork.CommitAsync();

        var memberDto = MapProjectMemberToDto(addedMember);

        _logger.LogInformation("End, user {Username} invited successfully to project {ProjectId}", inviteDto.Username, inviteDto.ProjectId);
        return TaskErrorResult<ProjectMemberDto>.Success(memberDto);
    }
   
   public async Task RemoveUserFromProjectAsync(Guid projectId, Guid userId, Guid currentUserId)
   {
       // Check if project exists
       if (!await _unitOfWork.ProjectRepository.ExistsAsync(projectId))
           throw new KeyNotFoundException($"Project with ID {projectId} not found");

       // Check if the current user has permission (is owner or admin)
       var currentUserMembership = await _unitOfWork.ProjectMemberRepository.GetByProjectAndUserIdAsync(projectId, currentUserId);
       if (currentUserMembership == null || 
           (currentUserMembership.Role != ProjectRole.Owner && currentUserMembership.Role != ProjectRole.Admin))
           throw new UnauthorizedAccessException("You don't have permission to remove users from this project");

       // Cannot remove the owner
       var memberToRemove = await _unitOfWork.ProjectMemberRepository.GetByProjectAndUserIdAsync(projectId, userId);
       if (memberToRemove == null)
           throw new KeyNotFoundException($"User with ID {userId} is not a member of this project");

       if (memberToRemove.Role == ProjectRole.Owner)
           throw new InvalidOperationException("Cannot remove the owner of the project");

       await _unitOfWork.ProjectMemberRepository.DeleteByProjectAndUserIdAsync(projectId, userId);
   }


    private async Task<ProjectDTO> MapToDto(ProjectEntity project)
    {
        var creator = await _unitOfWork.UserRepository.GetUserByIdAsync(project.CreatorUserId);
        
        return new ProjectDTO()
        {
            ProjectId = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            Creator = new UserDTO()
            {
                UserId = creator.UserId,
                FirstName = creator.FirstName,
                LastName = creator.LastName,
                Username = creator.Username,
                Email = creator.Email,
                Role = creator.Role
            }
        };
    }

    private ProjectMemberDto MapProjectMemberToDto(ProjectMemberEntity member)
    {
        return new ProjectMemberDto
        {
            ProjectMemberId = member.ProjectId,
            ProjectId = member.ProjectId,
            User = new UserDTO()
            {
                UserId = member.User.UserId,
                FirstName = member.User.FirstName,
                LastName = member.User.LastName,
                Username = member.User.Username,
                Email = member.User.Email,
                Role = member.User.Role
            },
            Role = member.Role.ToString(),
            JoinedAt = member.JoinedAt
        };
    }
}