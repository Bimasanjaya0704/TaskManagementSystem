using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Domain.Entities;

public class UserEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.User;

    public virtual List<TaskEntity> AssignedTasks { get; set; } = new();
    public virtual List<TaskEntity> ReviewedTasks { get; set; } = new();
}