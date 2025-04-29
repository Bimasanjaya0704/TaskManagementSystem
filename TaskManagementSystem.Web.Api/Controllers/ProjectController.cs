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

    [Authorize(Roles = "User,Admin")]
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

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, GetAllProjects - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var projectResponseDto = _mapper.Map<List<ProjectResponseDto>>(result.Data);

        _logger.LogInformation("End, GetAllProjects - Success, Retrieved {Count} projects", projectResponseDto.Count);
        return Ok(new ApiResponse<List<ProjectResponseDto>>(true, "Success", projectResponseDto));
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectById(int id)
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

    [Authorize(Roles = "User,Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] ProjectRequestDto projectRequestDto)
    {
        _logger.LogInformation("Start, CreateProject");

        var token = _tokenService.GetTokenFromHeader(Request);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("End, CreateProject - Failed: Token is missing");
            return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
        }

        var projectDto = _mapper.Map<ProjectDTO>(projectRequestDto);
        var result = await _projectService.AddProjectAsync(projectDto);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("End, CreateProject - Failed: {ErrorMessage}", result.ErrorMessage);
            return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
        }

        var projectResponseDto = _mapper.Map<ProjectResponseDto>(result.Data);

        _logger.LogInformation("End, CreateProject - Success: Created Project {ProjectId}", projectResponseDto.Id);
        return Ok(new ApiResponse<ProjectResponseDto>(true, "Success", projectResponseDto));
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectRequestDto projectRequestDto)
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

        _logger.LogInformation("End, UpdateProject - Success: Project updated {ProjectId}", projectResponseDto.Id);
        return Ok(new ApiResponse<ProjectResponseDto>(true, "Project updated successfully.", projectResponseDto));
    }

    [Authorize(Roles = "User,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
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

        _logger.LogInformation("End, DeleteProject - Success: Project deleted {ProjectId}", deletedTask.Id);
        return Ok(new ApiResponse<ProjectResponseDto>(true, "Project deleted successfully.", deletedTask));
    }
}