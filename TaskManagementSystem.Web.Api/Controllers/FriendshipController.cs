using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Web.Api.Models;
using TaskManagementSystem.Web.Api.Services;

namespace TaskManagementSystem.Web.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendshipController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;
        private readonly ILogger<FriendshipController> _logger;
        private readonly IMapper _mapper;
        private readonly TokenService _tokenService;

        public FriendshipController(IFriendshipService friendshipService, ILogger<FriendshipController> logger, IMapper mapper, TokenService tokenService)
        {
            _friendshipService = friendshipService;
            _logger = logger;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost("send-request")]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequestDto friendRequestDto)
        {
            _logger.LogInformation("Start, SendFriendRequest");

            var token = _tokenService.GetTokenFromHeader(Request);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token is missing.");
                return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
            }

            var result = await _friendshipService.SendFriendRequestAsync(friendRequestDto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
            }

            var friendshipDto = result.Data;

            _logger.LogInformation("End, SendFriendRequest - Success");
            return Ok(new ApiResponse<FriendshipDto>(true, "Friend request sent successfully.", friendshipDto));
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost("respond-request")]
        public async Task<IActionResult> RespondToFriendRequest([FromBody] RespondToFriendRequestDto respondDto)
        {
            _logger.LogInformation("Start, RespondToFriendRequest");

            var token = _tokenService.GetTokenFromHeader(Request);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token is missing.");
                return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
            }

            var currentUserId = Guid.Parse(token); // Assuming token contains user ID for simplicity
            var result = await _friendshipService.RespondToFriendRequestAsync(respondDto, currentUserId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
            }

            var friendshipDto = result.Data;

            _logger.LogInformation("End, RespondToFriendRequest - Success");
            return Ok(new ApiResponse<FriendshipDto>(true, "Response to friend request processed successfully.", friendshipDto));
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("pending-requests/{userId}")]
        public async Task<IActionResult> GetPendingFriendRequests(Guid userId)
        {
            _logger.LogInformation("Start, GetPendingFriendRequests");

            var token = _tokenService.GetTokenFromHeader(Request);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token is missing.");
                return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
            }

            var result = await _friendshipService.GetPendingFriendRequestsAsync(userId);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(new ApiResponse<string>(false, result.ErrorMessage, null));
            }

            var friendshipDtos = result.Data.ToList();

            _logger.LogInformation("End, GetPendingFriendRequests - Success");
            return Ok(new ApiResponse<IEnumerable<FriendshipDto>>(true, "Pending requests fetched successfully.", friendshipDtos));
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("are-friends/{userId1}/{userId2}")]
        public async Task<IActionResult> AreFriends(Guid userId1, Guid userId2)
        {
            _logger.LogInformation("Start, AreFriends");

            var token = _tokenService.GetTokenFromHeader(Request);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token is missing.");
                return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
            }

            var result = await _friendshipService.AreFriendsAsync(userId1, userId2);

            _logger.LogInformation("End, AreFriends - {Result}", result ? "Users are friends" : "Users are not friends");
            return Ok(new ApiResponse<bool>(true, result ? "Users are friends." : "Users are not friends.", result));
        }

        [Authorize(Roles = "User,Admin")]
        [HttpDelete("remove-friend/{userId1}/{userId2}")]
        public async Task<IActionResult> RemoveFriendship(Guid userId1, Guid userId2)
        {
            _logger.LogInformation("Start, RemoveFriendship");

            var token = _tokenService.GetTokenFromHeader(Request);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token is missing.");
                return Unauthorized(new ApiResponse<string>(false, "Token is missing", null));
            }

            var result = await _friendshipService.RemoveFriendshipAsync(userId1, userId2);

            if (!result)
            {
                _logger.LogWarning("Failed: Friendship not found.");
                return BadRequest(new ApiResponse<string>(false, "Friendship not found.", null));
            }

            _logger.LogInformation("End, RemoveFriendship - Success");
            return Ok(new ApiResponse<string>(true, "Friendship removed successfully.", null));
        }
    }
}
