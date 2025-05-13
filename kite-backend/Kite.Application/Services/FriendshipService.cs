using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kite.Application.Services;

public class FriendshipService(
    IUserAccessor userAccessor,
    UserManager<ApplicationUser> userManager,
    IFriendshipRepository friendshipRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository) : IFriendshipService
{
    private async Task<string?> IdentifyCurrentUser()
    {
        var currentUserResult = await userAccessor.GetCurrentUserIdAsync();
        if (!currentUserResult.IsSuccess)
        {
            throw new UnauthorizedAccessException("Failed to identify the current user");
        }

        return currentUserResult.Value;
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

            var targetUser = await userManager.FindByIdAsync(targetUserId);
            if (targetUser == null)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.UserNotFound", "Target user does not exist"));
            }

            var existingFriendship =
                await friendshipRepository.CheckIfFrienshipExists(currentUserId, targetUserId);
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

                    await friendshipRepository.UpdateAsync(existingFriendship);
                    return Result<string>.Success("Friend request sent successfully");
                }
            }

            var newFriendship = new Friendship
            {
                Id = Guid.NewGuid(),
                SenderId = currentUserId,
                ReceiverId = targetUserId,
                Status = FriendRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await friendshipRepository.InsertAsync(newFriendship);
            
            await unitOfWork.SaveChangesAsync();

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

            var friendship = await friendshipRepository.GetByIdAsync(requestId);
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

            await friendshipRepository.UpdateAsync(friendship);

            await unitOfWork.SaveChangesAsync();

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

            var friendship = await friendshipRepository.GetByIdAsync(requestId);
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

            await friendshipRepository.UpdateAsync(friendship);

            await unitOfWork.SaveChangesAsync();

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

            var friendship = await friendshipRepository.GetByIdAsync(requestId);
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

            await friendshipRepository.UpdateAsync(friendship);
            
            await unitOfWork.SaveChangesAsync();

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
                await friendshipRepository.GetPendingReceivedRequestsAsync(currentUserId);

            var requestModels = new List<FriendRequestModel>();

            foreach (var request in pendingRequests)
            {
                var sender = await userRepository.GetByIdAsync(request.SenderId);

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
                await friendshipRepository.GetPendingSentRequestsAsync(currentUserId);

            var requestModels = new List<FriendRequestModel>();

            foreach (var request in pendingSentRequests)
            {
                var receiver = await userRepository.GetByIdAsync(request.ReceiverId);

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
                        CreatedAt = request.CreatedAt,
                        Status = request.Status,
                        TimeElapsed = GetTimeElapsedString(request.CreatedAt)
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

    private static string GetTimeElapsedString(DateTime requestTime)
    {
        var currentTime = DateTime.UtcNow;
        var timeSpan = currentTime - requestTime;

        return timeSpan switch
        {
            var ts when ts.TotalDays > 365 =>
                (int)(ts.TotalDays / 365) == 1
                    ? "1 year ago"
                    : $"{(int)(ts.TotalDays / 365)} years ago",

            var ts when ts.TotalDays > 30 =>
                (int)(ts.TotalDays / 30) == 1
                    ? "1 month ago"
                    : $"{(int)(ts.TotalDays / 30)} months ago",

            var ts when ts.TotalDays > 7 =>
                (int)(ts.TotalDays / 7) == 1
                    ? "1 week ago"
                    : $"{(int)(ts.TotalDays / 7)} weeks ago",

            var ts when ts.TotalDays >= 1 =>
                (int)ts.TotalDays == 1 ? "yesterday" : $"{(int)ts.TotalDays} days ago",

            var ts when ts.TotalHours >= 1 =>
                (int)ts.TotalHours == 1 ? "an hour ago" : $"{(int)ts.TotalHours} hours ago",

            var ts when ts.TotalMinutes >= 1 =>
                (int)ts.TotalMinutes == 1 ? "a minute ago" : $"{(int)ts.TotalMinutes} minutes ago",

            var ts when ts.TotalSeconds >= 30 => "less than a minute ago",

            _ => "just now"
        };
    }

    public async Task<Result<IEnumerable<UserModel>>> GetFriendsAsync()
    {
        try
        {
            var currentUserId = await IdentifyCurrentUser();
            var friendships =
                await friendshipRepository.GetAcceptedFriendshipsForUserAsync(currentUserId);

            var friendUserIds = new HashSet<string>();
            foreach (var friendship in friendships)
            {
                if (friendship.SenderId == currentUserId)
                {
                    friendUserIds.Add(friendship.ReceiverId);
                }
                else if (friendship.ReceiverId == currentUserId)
                {
                    friendUserIds.Add(friendship.SenderId);
                }
            }

            var friendModels = new List<UserModel>();
            foreach (var friendId in friendUserIds)
            {
                var friendUser = await userRepository.GetByIdAsync(friendId);
                if (friendUser != null)
                {
                    friendModels.Add(new UserModel
                    {
                        Id = friendUser.Id,
                        UserName = friendUser.UserName,
                        FirstName = friendUser.FirstName,
                        LastName = friendUser.LastName,
                        Email = friendUser.Email,
                        ImageUrl = friendUser.ImageUrl,
                    });
                }
            }

            return Result<IEnumerable<UserModel>>.Success(friendModels);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<UserModel>>.Failure(
                new Error("Friends.Exception", $"Failed to retrieve friends: {ex.Message}"));
        }
    }
}