namespace TaskManagementSystem.Application.DTOs;

public class RespondToFriendRequestDto
{
    public Guid FriendshipId { get; set; }
    public bool Accept { get; set; }
}