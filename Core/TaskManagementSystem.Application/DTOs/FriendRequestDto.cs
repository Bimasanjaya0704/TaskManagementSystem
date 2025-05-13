namespace TaskManagementSystem.Application.DTOs;

public class FriendRequestDto
{
    public Guid SenderId { get; set; }
    public string ReceiverUsername { get; set; }
}