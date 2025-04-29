using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.Domain.Entities;

public class TaskEntity 
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public TaskStatus Status { get; set; }  
    
    public int AssignedToUserId { get; set; }
    public UserEntity AssignedToUser { get; set; }

    public int? ReviewedByUserId { get; set; }  
    public UserEntity? ReviewedByUser { get; set; }
    public int ProjectId { get; set; }
    public ProjectEntity Project { get; set; }
}