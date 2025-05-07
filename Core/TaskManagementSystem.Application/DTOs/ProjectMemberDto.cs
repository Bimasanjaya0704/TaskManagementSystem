namespace TaskManagementSystem.Application.DTOs;

public class ProjectMemberDto
{
    public Guid ProjectMemberId { get; set; }
    public Guid ProjectId { get; set; }
    public UserDTO User { get; set; }
    public string Role { get; set; }
    public DateTime JoinedAt { get; set; }
}