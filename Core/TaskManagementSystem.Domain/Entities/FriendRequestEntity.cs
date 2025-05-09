using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Domain.Entities;

[Table("FriendRequest")]
public class FriendRequestEntity
{
    [Key] 
    public Guid FriendshipId { get; set; } = Guid.NewGuid();

    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    
    public FriendshipStatus Status { get; set; }
    public bool IsAccepted { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    
    // Navigation properties - match the names in UserEntity
    [ForeignKey("SenderId")]
    public virtual UserEntity Sender { get; set; }
    
    [ForeignKey("ReceiverId")]
    public virtual UserEntity Receiver { get; set; }
    
    public FriendRequestEntity()
    {
        Status = FriendshipStatus.Pending;
        RequestedAt = DateTime.UtcNow;
    }
}