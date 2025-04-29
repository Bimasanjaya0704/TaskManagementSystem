namespace TaskManagementSystem.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public List<Task> Tasks { get; set; } 
}