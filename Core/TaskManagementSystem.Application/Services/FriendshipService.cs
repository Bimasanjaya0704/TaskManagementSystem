using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Enum;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Result;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enum;
using TaskManagementSystem.Domain.Interfaces;

namespace TaskManagementSystem.Application.Services;

public class FriendshipService : IFriendshipService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<FriendshipService> _logger;

    public FriendshipService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<FriendshipService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<TaskErrorResult<FriendshipDto>> GetFriendshipByIdAsync(Guid friendshipId)
    {
        _logger.LogInformation("Start, Fetching friendship with ID: {FriendshipId}", friendshipId);

        var friendship = await _unitOfWork.FriendRequestRepository.GetByIdAsync(friendshipId);
        if (friendship == null)
        {
            _logger.LogWarning("Friendship with ID {FriendshipId} not found.", friendshipId);
            return TaskErrorResult<FriendshipDto>.Failure(TaskErrorType.ErrorFriendshipNotFound, "Friendship not found.");
        }

        var friendshipDto = _mapper.Map<FriendshipDto>(friendship);
        _logger.LogInformation("End, Fetching friendship with ID: {FriendshipId}", friendshipId);
        return TaskErrorResult<FriendshipDto>.Success(friendshipDto);
    }

    
    public async Task<TaskErrorResult<FriendshipDto>> SendFriendRequestAsync(FriendRequestDto friendRequestDto)
    {
        _logger.LogInformation("Start, Sending friend request from {RequesterId} to {AddresseeUsername}", 
            friendRequestDto.SenderId, friendRequestDto.ReceiverUsername);

        // Check if requester exists
        if (!await _unitOfWork.UserRepository.ExistsAsync(friendRequestDto.SenderId))
        {
            _logger.LogWarning("User with ID {RequesterId} not found.", friendRequestDto.SenderId);
            return TaskErrorResult<FriendshipDto>.Failure(TaskErrorType.ErrorUserNotFound, "Requester not found.");
        }

        // Find addressee by username
        var addressee = await _unitOfWork.UserRepository.GetByUsernameAsync(friendRequestDto.ReceiverUsername);
        if (addressee == null)
        {
            _logger.LogWarning("User with username {AddresseeUsername} not found.", friendRequestDto.ReceiverUsername);
            return TaskErrorResult<FriendshipDto>.Failure(TaskErrorType.ErrorUserNotFound, "Addressee not found.");
        }

        // Cannot send friend request to self
        if (friendRequestDto.SenderId == addressee.UserId)
        {
            _logger.LogWarning("Cannot send friend request to yourself.");
            return TaskErrorResult<FriendshipDto>.Failure(TaskErrorType.ErrorValidation, "Cannot send friend request to yourself.");
        }

        // Check if friendship already exists
        if (await _unitOfWork.FriendRequestRepository.ExistsByUserIdsAsync(friendRequestDto.SenderId, addressee.UserId))
        {
            _logger.LogWarning("Friendship or request already exists between these users.");
            return TaskErrorResult<FriendshipDto>.Failure(TaskErrorType.ErrorFriendshipAlreadyExist, "Friendship already exists.");
        }

        var friendship = new FriendRequestEntity()
        {
            SenderId = friendRequestDto.SenderId,
            ReceiverId = addressee.UserId,
            Status = FriendshipStatus.Pending
        };

        var addedFriendship = await _unitOfWork.FriendRequestRepository.AddAsync(friendship);

        // Load navigation properties for DTO mapping
        var requester = await _unitOfWork.UserRepository.GetUserByIdAsync(addedFriendship.SenderId);
        addedFriendship.Sender = requester;

        addedFriendship.Receiver = addressee;

        var friendshipDtoResult = _mapper.Map<FriendshipDto>(addedFriendship);

        _logger.LogInformation("End, Sending friend request from {RequesterId} to {AddresseeUsername}", 
            friendRequestDto.SenderId, friendRequestDto.ReceiverUsername);
        return TaskErrorResult<FriendshipDto>.Success(friendshipDtoResult);
    }
    
    public async Task<TaskErrorResult<FriendshipDto>> RespondToFriendRequestAsync(RespondToFriendRequestDto respondDto, Guid currentUserId)
    {
        _logger.LogInformation("Start, Responding to friend request with ID: {FriendshipId}", respondDto.FriendshipId);

        var friendship = await _unitOfWork.FriendRequestRepository.GetByIdAsync(respondDto.FriendshipId);
        if (friendship == null)
        {
        _logger.LogWarning("Friendship with ID {FriendshipId} not found.", respondDto.FriendshipId);
        return TaskErrorResult<FriendshipDto>.Failure(TaskErrorType.ErrorFriendshipNotFound, "Friendship not found.");
        }

        // Only the addressee can respond to the request
        if (friendship.ReceiverId != currentUserId)
        {
        _logger.LogWarning("User {CurrentUserId} is not the receiver of this request.", currentUserId);
        return TaskErrorResult<FriendshipDto>.Failure(TaskErrorType.ErrorValidation, "You are not the receiver.");
        }

        // Can only respond to pending requests
        if (friendship.Status != FriendshipStatus.Pending)
        {
        _logger.LogWarning("This friend request has already been processed.");
        return TaskErrorResult<FriendshipDto>.Failure(TaskErrorType.ErrorInvalidOperation, "This friend request has already been processed.");
        }

        friendship.Status = respondDto.Accept ? FriendshipStatus.Accepted : FriendshipStatus.Rejected;

        if (respondDto.Accept)
        friendship.AcceptedAt = DateTime.UtcNow;

        await _unitOfWork.FriendRequestRepository.UpdateAsync(friendship);

        // Load navigation properties for DTO mapping
        var sender = await _unitOfWork.UserRepository.GetUserByIdAsync(friendship.SenderId);
        friendship.Sender = sender;

        var receiver = await _unitOfWork.UserRepository.GetUserByIdAsync(friendship.ReceiverId);
        friendship.Receiver = receiver;

        var friendshipDtoResult = _mapper.Map<FriendshipDto>(friendship);

        _logger.LogInformation("End, Responding to friend request with ID: {FriendshipId}", respondDto.FriendshipId);
        return TaskErrorResult<FriendshipDto>.Success(friendshipDtoResult);
    }
    
    public async Task<TaskErrorResult<IEnumerable<FriendshipDto>>> GetPendingFriendRequestsAsync(Guid userId)
    {
        _logger.LogInformation("Start, Fetching pending friend requests for User ID: {UserId}", userId);

        var pendingRequests = await _unitOfWork.FriendRequestRepository.GetPendingRequestsForUserAsync(userId);
        if (pendingRequests == null || !pendingRequests.Any())
        {
            _logger.LogWarning("No pending friend requests found for User ID: {UserId}", userId);
            return TaskErrorResult<IEnumerable<FriendshipDto>>.Failure(TaskErrorType.ErrorNoPendingRequests, "No pending requests.");
        }

        var friendshipDtos = pendingRequests.Select(friendship => _mapper.Map<FriendshipDto>(friendship));
        _logger.LogInformation("End, Fetching pending friend requests for User ID: {UserId}", userId);
        return TaskErrorResult<IEnumerable<FriendshipDto>>.Success(friendshipDtos);
    }

    public async Task<bool> AreFriendsAsync(Guid user1Id, Guid user2Id)
    {
        _logger.LogInformation("Start, Checking if User {User1Id} and User {User2Id} are friends", user1Id, user2Id);

        var friendship = await _unitOfWork.FriendRequestRepository.GetByUserIdsAsync(user1Id, user2Id);
        if (friendship != null && friendship.Status == FriendshipStatus.Accepted)
        {
            _logger.LogInformation("Users {User1Id} and {User2Id} are friends.", user1Id, user2Id);
            return true;
        }

        _logger.LogInformation("Users {User1Id} and {User2Id} are not friends.", user1Id, user2Id);
        return false;
    }
    
    public async Task<bool> RemoveFriendshipAsync(Guid user1Id, Guid user2Id)
    {
        _logger.LogInformation("Start, Removing friendship between User {User1Id} and User {User2Id}", user1Id, user2Id);

        var friendship = await _unitOfWork.FriendRequestRepository.GetByUserIdsAsync(user1Id, user2Id);
        if (friendship == null)
        {
            _logger.LogWarning("Friendship not found between User {User1Id} and User {User2Id}.", user1Id, user2Id);
            return false;
        }

        await _unitOfWork.FriendRequestRepository.DeleteAsync(friendship.FriendshipId);
        _logger.LogInformation("End, Removing friendship between User {User1Id} and User {User2Id}", user1Id, user2Id);
        return true;
    }
}