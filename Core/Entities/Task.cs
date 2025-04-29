using Core.Enum;

namespace Core.Entities;

public class Task
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskStatusProgress Status { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
}