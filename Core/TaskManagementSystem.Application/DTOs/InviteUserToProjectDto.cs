using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Application.DTOs;

public class InviteUserToProjectDto
{
    public Guid ProjectId { get; set; }
    public string Username { get; set; }
    public ProjectRole ProjectRole { get; set; }
}