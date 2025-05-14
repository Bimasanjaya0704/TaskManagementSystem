using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Web.Api.DTOs;
using TaskManagementSystem.Web.Api.Models;
using TaskManagementSystem.Web.Api.Services;

namespace TaskManagementSystem.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectController> _logger;
    private readonly TokenService _tokenService;
    private readonly IMapper _mapper;

    public ProjectController(IProjectService projectService, ILogger<ProjectController> logger, TokenService tokenService, IMapper mapper)
    {
        _projectService = projectService;
        _logger = logger;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [Authorize(Roles = "User,Admin,SuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        _logger.LogInformation("Start, GetAllProjects");

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("End, GetAllProjects - Failed: Token is missing");
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }
        var result = await _projectService.GetAllProjectAsync();
        
        if (result.Data == null || !result.Data.Any())
        {
            _logger.LogInformation("End, GetAllProjects - No projects found.");
            return Ok(new ApiResponse<List<ProjectResponseDto>>(true, "No projects found.", null)); 
        }

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, GetAllProjects - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var projectResponseDto = _mapper.Map<List<ProjectResponseDto>>(result.Data);

        _logger.LogInformation("End, GetAllProjects - Success, Retrieved {Count} projects", projectResponseDto.Count);
        return Ok(new ApiResponse<List<ProjectResponseDto>>(true, "Success", projectResponseDto));
    }

    [Authorize(Roles = "User,Admin,SuperAdmin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectById(Guid id)
    {
        _logger.LogInformation("Start, GetTaskById: {TaskId}", id);

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("End, GetTaskById - Failed: Token is missing");
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var result = await _projectService.GetProjectByIdAsync(id);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, GetTaskById - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var projectResponseDto = _mapper.Map<ProjectResponseDto>(result.Data);

        _logger.LogInformation("End, GetTaskById - Success: Found Task {TaskId}", id);
        return Ok(new ApiResponse<ProjectResponseDto>(true, "Success", projectResponseDto));
    }

    [Authorize(Roles = "User,Admin,SuperAdmin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateProject([FromBody] ProjectRequestDto projectRequestDto)
    {
        _logger.LogInformation("Start, CreateProject");

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("End, CreateProject - Failed: Token is missing");
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("End, CreateProject - Failed: Invalid or missing User ID in token");
            return Unauthorized(new ApiResponse<string>(false, "Invalid or missing User ID", null));
        }

        var projectDto = _mapper.Map<ProjectDTO>(projectRequestDto);
        projectDto.CreatorUserId = userId;
        var result = await _projectService.CreateProjectAsync(projectDto);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, CreateProject - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var projectResponseDto = _mapper.Map<ProjectResponseDto>(result.Data);

        _logger.LogInformation("End, CreateProject - Success: Created Project {ProjectId}", projectResponseDto.ProjectId);
        return Ok(new ApiResponse<ProjectResponseDto>(true, "Success", projectResponseDto));
    }

    [Authorize(Roles = "User,Admin,SuperAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] ProjectRequestDto projectRequestDto)
    {
        _logger.LogInformation("Start, UpdateProject: {ProjectId}", id);

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("End, UpdateProject - Failed: Token is missing");
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        if (projectRequestDto == null)
        {
            _logger.LogWarning("End, UpdateProject - Failed: Invalid request data.");
            return BadRequest(new ApiResponse<string>(false, "Invalid request data.", null));
        }

        var projectDto = _mapper.Map<ProjectDTO>(projectRequestDto);

        var result = await _projectService.UpdateProjectAsync(id, projectDto);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, UpdateProject - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var projectResponseDto = _mapper.Map<ProjectResponseDto>(result.Data);

        _logger.LogInformation("End, UpdateProject - Success: Project updated {ProjectId}", projectResponseDto.ProjectId);
        return Ok(new ApiResponse<ProjectResponseDto>(true, "Project updated successfully.", projectResponseDto));
    }

    [Authorize(Roles = "User,Admin,SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        _logger.LogInformation("Start, DeleteProject: {ProjectId}", id);

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("End, DeleteProject - Failed: Token is missing");
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var result = await _projectService.DeleteProjectAsync(id);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, DeleteProject - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var deletedTask = _mapper.Map<ProjectResponseDto>(result.Data);

        _logger.LogInformation("End, DeleteProject - Success: Project deleted {ProjectId}", deletedTask.ProjectId);
        return Ok(new ApiResponse<ProjectResponseDto>(true, "Project deleted successfully.", deletedTask));
    }
    
    [Authorize(Roles = "User,Admin,SuperAdmin")]
    [HttpGet("{projectId}/members")]
    public async Task<IActionResult> GetProjectMembers(Guid projectId)
    {
        _logger.LogInformation("Start, GetProjectMembers: {ProjectId}", projectId);

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var result = await _projectService.GetProjectMembersAsync(projectId);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        return Ok(new ApiResponse<IEnumerable<ProjectMemberDto>>(true, "Success", result.Data));
    }
    
    [Authorize(Roles = "User,Admin,SuperAdmin")]
    [HttpPost("{projectId}/invite")]
    public async Task<IActionResult> InviteUserToProject(Guid projectId, [FromBody] InviteUserToProjectDto inviteDto)
    {
        _logger.LogInformation("Start, InviteUserToProject: {ProjectId}", projectId);

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var inviterUserId = _tokenService.GetUserIdFromToken(token);
        if (inviterUserId == Guid.Empty)
        {
            return Unauthorized(new ApiResponse<string>(false, "Invalid user ID in token.", null));
        }

        if (inviteDto == null || inviteDto.ProjectId != projectId)
        {
            return BadRequest(new ApiResponse<string>(false, "Invalid invite data.", null));
        }

        var result = await _projectService.InviteUserToProjectAsync(inviteDto, inviterUserId);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        return Ok(new ApiResponse<ProjectMemberDto>(true, "User invited successfully.", result.Data));
    }
    
    [Authorize(Roles = "User,Admin,SuperAdmin")]
    [HttpDelete("{projectId}/remove-member/{userId}")]
    public async Task<IActionResult> RemoveUserFromProject(Guid projectId, Guid userId)
    {
        _logger.LogInformation("Start, RemoveUserFromProject: {ProjectId}, {UserId}", projectId, userId);

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var currentUserId = _tokenService.GetUserIdFromToken(token);
        if (currentUserId == Guid.Empty)
        {
            return Unauthorized(new ApiResponse<string>(false, "Invalid user ID in token.", null));
        }

        try
        {
            await _projectService.RemoveUserFromProjectAsync(projectId, userId, currentUserId);
            return Ok(new ApiResponse<string>(true, "User removed from project.", null));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<string>(false, ex.Message, null));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<string>(false, ex.Message, null));
        }
    }

    

}