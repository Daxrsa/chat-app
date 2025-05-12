using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kite.Application.Services;

public class FriendshipService : IFriendshipService
{
    private readonly IUserAccessor _userAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FriendshipService(IUserAccessor userAccessor, UserManager<ApplicationUser> userManager,
        IFriendshipRepository friendshipRepository, IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _userAccessor = userAccessor;
        _userManager = userManager;
        _friendshipRepository = friendshipRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<Result<string>> SendFriendRequestAsync(string targetUserId)
    {
        try
        {
            var currentUserId = await IdentifyCurrentUser();

            if (string.IsNullOrEmpty(targetUserId))
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.InvalidTarget", "Target user ID cannot be empty"));
            }

            if (currentUserId == targetUserId)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.SelfRequest",
                        "You cannot send a friend request to yourself"));
            }

            var targetUser = await _userManager.FindByIdAsync(targetUserId);
            if (targetUser == null)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.UserNotFound", "Target user does not exist"));
            }

            var existingFriendship =
                await _friendshipRepository.CheckIfFrienshipExists(currentUserId, targetUserId);
            if (existingFriendship != null)
            {
                if (existingFriendship.Status == FriendRequestStatus.Accepted)
                {
                    return Result<string>.Failure(
                        new Error("FriendRequest.AlreadyFriends",
                            "You are already friends with this user"));
                }

                if (existingFriendship.Status == FriendRequestStatus.Pending &&
                    existingFriendship.SenderId == currentUserId)
                {
                    return Result<string>.Failure(
                        new Error("FriendRequest.AlreadySent",
                            "You have already sent a friend request to this user"));
                }

                if (existingFriendship.Status == FriendRequestStatus.Pending &&
                    existingFriendship.ReceiverId == currentUserId)
                {
                    return Result<string>.Failure(
                        new Error("FriendRequest.AlreadyReceived",
                            "This user has already sent you a friend request. You can accept it instead."));
                }

                // Previously rejected/withdrawn request - could potentially allow resending
                if (existingFriendship.Status == FriendRequestStatus.Rejected ||
                    existingFriendship.Status == FriendRequestStatus.Withdrawn)
                {
                    // Update existing record instead of creating new
                    existingFriendship.Status = FriendRequestStatus.Pending;
                    existingFriendship.ResendRequestTime = DateTime.UtcNow;

                    await _friendshipRepository.UpdateAsync(existingFriendship);
                    return Result<string>.Success("Friend request sent successfully");
                }
            }

            var newFriendship = new Friendship
            {
                Id = Guid.NewGuid(),
                SenderId = currentUserId,
                ReceiverId = targetUserId,
                Status = FriendRequestStatus.Pending,
                RequestSentTime = DateTime.UtcNow
            };

            await _friendshipRepository.InsertAsync(newFriendship);

            // Optionally, send notification to target user
            // await _notificationService.SendFriendRequestNotificationAsync(targetUserId, currentUserId);

            return Result<string>.Success("Friend request sent successfully");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(
                new Error("FriendRequest.Exception",
                    $"Failed to send friend request: {ex.Message}"));
        }
    }

    public async Task<Result<string>> AcceptFriendRequestAsync(Guid requestId)
    {
        try
        {
            var currentUserId = await IdentifyCurrentUser();

            if (requestId == Guid.Empty)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.InvalidId", "Invalid friend request ID"));
            }

            var friendship = await _friendshipRepository.GetByIdAsync(requestId);
            if (friendship == null)
            {
                return Result<string>.Success("No existing friendship found");
            }

            if (friendship.Status != FriendRequestStatus.Pending)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.InvalidStatus",
                        $"This request cannot be accepted because it's not of pending status"));
            }

            if (friendship.ReceiverId != currentUserId)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.Unauthorized",
                        "You cannot accept this request as it was not sent to you"));
            }

            friendship.Status = FriendRequestStatus.Accepted;

            await _unitOfWork.SaveChangesAsync();

            // Optionally, create notification for the sender
            // await _notificationService.SendFriendRequestAcceptedNotificationAsync(friendship.SenderId, currentUserId);

            return Result<string>.Success("Friend request accepted successfully");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(
                new Error("FriendRequest.Exception",
                    $"Failed to accept friend request: {ex.Message}"));
        }
    }

    public async Task<Result<string>> RejectFriendRequestAsync(Guid requestId)
    {
        try
        {
            var currentUserId = await IdentifyCurrentUser();

            if (requestId == Guid.Empty)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.InvalidId", "Invalid friend request ID"));
            }

            var friendship = await _friendshipRepository.GetByIdAsync(requestId);
            if (friendship == null)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.NotFound", "Friend request not found"));
            }

            if (friendship.Status != FriendRequestStatus.Pending)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.InvalidStatus",
                        $"This request cannot be rejected because it's not of pending status"));
            }

            if (friendship.ReceiverId != currentUserId)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.Unauthorized",
                        "You cannot reject this request as it was not sent to you"));
            }

            friendship.Status = FriendRequestStatus.Rejected;

            await _friendshipRepository.UpdateAsync(friendship);

            return Result<string>.Success("Friend request rejected successfully");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(
                new Error("FriendRequest.Exception",
                    $"Failed to reject friend request: {ex.Message}"));
        }
    }

    public async Task<Result<string>> WithdrawFriendRequestAsync(Guid requestId)
    {
        try
        {
            var currentUserId = await IdentifyCurrentUser();

            if (requestId == Guid.Empty)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.InvalidId", "Invalid friend request ID"));
            }

            var friendship = await _friendshipRepository.GetByIdAsync(requestId);
            if (friendship == null)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.NotFound", "Friend request not found"));
            }

            if (friendship.Status != FriendRequestStatus.Pending)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.InvalidStatus",
                        $"This request cannot be withdrawn because it has status: {friendship.Status}"));
            }

            if (friendship.SenderId != currentUserId)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.Unauthorized",
                        "You cannot withdraw this request as you did not send it"));
            }

            friendship.Status = FriendRequestStatus.Withdrawn;

            await _friendshipRepository.UpdateAsync(friendship);

            return Result<string>.Success("Friend request withdrawn successfully");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(
                new Error("FriendRequest.Exception",
                    $"Failed to withdraw friend request: {ex.Message}"));
        }
    }

    public async Task<Result<IEnumerable<FriendRequestModel>>> GetPendingReceivedRequestsAsync()
    {
        try
        {
            var currentUserId = await IdentifyCurrentUser();

            var pendingRequests =
                await _friendshipRepository.GetPendingReceivedRequestsAsync(currentUserId);

            var requestModels = new List<FriendRequestModel>();

            foreach (var request in pendingRequests)
            {
                var sender = await _userRepository.GetByIdAsync(request.SenderId);

                if (sender != null)
                {
                    requestModels.Add(new FriendRequestModel
                    {
                        Id = request.Id,
                        SenderId = request.SenderId,
                        SenderFirstName = sender.FirstName,
                        SenderLastName = sender.LastName,
                        SenderUsername = sender.UserName,
                        SenderImageUrl = sender.ImageUrl,
                        Status = request.Status
                    });
                }
            }

            return Result<IEnumerable<FriendRequestModel>>.Success(requestModels);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<FriendRequestModel>>.Failure(
                new Error("FriendRequest.Exception",
                    $"Failed to retrieve pending friend requests: {ex.Message}"));
        }
    }

    public async Task<Result<IEnumerable<FriendRequestModel>>> GetPendingSentRequestsAsync()
    {
        try
        {
            var currentUserId = await IdentifyCurrentUser();

            var pendingSentRequests =
                await _friendshipRepository.GetPendingSentRequestsAsync(currentUserId);

            var requestModels = new List<FriendRequestModel>();

            foreach (var request in pendingSentRequests)
            {
                var receiver = await _userRepository.GetByIdAsync(request.ReceiverId);

                if (receiver != null)
                {
                    requestModels.Add(new FriendRequestModel
                    {
                        Id = request.Id,
                        ReceiverId = request.ReceiverId,
                        ReceiverFirstName = receiver.FirstName,
                        ReceiverLastName = receiver.LastName,
                        ReceiverUsername = receiver.UserName,
                        ReceiverImageUrl = receiver.ImageUrl,
                        SentAt = request.CreatedAt,
                        Status = request.Status,
                        // Add time elapsed since request was sent
                        TimeElapsed = GetTimeElapsedString(request.CreatedAt, DateTime.UtcNow)
                    });
                }
            }

            return Result<IEnumerable<FriendRequestModel>>.Success(requestModels);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<FriendRequestModel>>.Failure(
                new Error("FriendRequest.Exception",
                    $"Failed to retrieve pending sent friend requests: {ex.Message}"));
        }
    }

    public async Task<Result<IEnumerable<UserModel>>> GetFriendsAsync()
    {
        throw new NotImplementedException();
    }

    private async Task<string?> IdentifyCurrentUser()
    {
        var currentUserResult = await _userAccessor.GetCurrentUserIdAsync();
        if (!currentUserResult.IsSuccess)
        {
            throw new UnauthorizedAccessException("Failed to identify the current user");
        }

        return currentUserResult.Value;
    }
}