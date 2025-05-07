using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Application.DTOs;

public class FriendshipDto
{
    public Guid FriendshipId { get; set; }
    public UserDTO SenderId { get; set; }
    public UserDTO ReceiverId { get; set; }
    public FriendshipStatus Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
}