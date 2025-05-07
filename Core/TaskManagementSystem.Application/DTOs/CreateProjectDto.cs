namespace TaskManagementSystem.Application.DTOs;

public class CreateProjectDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid CreatorUserId { get; set; }
}