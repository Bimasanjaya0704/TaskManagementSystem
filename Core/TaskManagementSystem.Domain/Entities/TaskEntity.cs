namespace TaskManagementSystem.Domain.Entities;

public class TaskEntity 
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public TaskStatus Status { get; set; }  
    public int AssignedToUserId { get; set; }
    public User AssignedToUser { get; set; }

    public int? ReviewedByUserId { get; set; }  
    public User ReviewedByUser { get; set; }
}