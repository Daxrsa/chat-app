using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Application.Utilities;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kite.Application.Services;

public class FriendRequestService(
    IUserAccessor userAccessor,
    UserManager<ApplicationUser> userManager,
    IFriendRequestRepository friendRequestRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    INotificationService notificationService,
    IFriendshipRepository friendshipRepository) : IFriendRequestService
{
    public async Task<Result<string>> SendFriendRequestAsync(string targetUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = userAccessor.GetCurrentUserId();

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
                await friendshipRepository.CheckIfFrienshipExists(currentUserId, targetUserId,
                    cancellationToken);

            if (existingFriendship is not null)
            {
                return Result<string>.Success("You are already friends with this user.");
            }

            var newFriendship = new FriendRequest
            {
                Id = Guid.NewGuid(),
                SenderId = currentUserId,
                ReceiverId = targetUserId,
                Status = FriendRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await friendRequestRepository.InsertAsync(newFriendship, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var notificationModel = new NotificationModel
            {
                SenderId = currentUserId,
                ReceiverId = targetUserId,
                Message =
                    $"You have received a friend request from {userAccessor.GetUserFirstName()} {userAccessor.GetUserLastName()}",
                Type = NotificationType.FriendRequest,
                IsRead = false,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await notificationService.CreateNotificationAsync(notificationModel, cancellationToken);

            return Result<string>.Success();
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(
                new Error("FriendRequest.Exception",
                    $"Failed to send friend request: {ex.Message}"));
        }
    }

    public async Task<Result<string>> AcceptFriendRequestAsync(Guid requestId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (requestId == Guid.Empty)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.InvalidId", "Invalid friend request ID"));
            }

            var friendshipRequest =
                await friendRequestRepository.GetByIdAsync(requestId, cancellationToken);
            if (friendshipRequest.Status != FriendRequestStatus.Pending)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.InvalidStatus",
                        $"This request cannot be accepted because it's not of pending status"));
            }

            friendshipRequest.Status = FriendRequestStatus.Accepted;

            var newFriendship = new Friendship
            {
                Id = Guid.NewGuid(),
                FriendRequestId = friendshipRequest.Id,
                FriendRequest = friendshipRequest,
                CreatedAt = DateTime.UtcNow
            };

            friendshipRequest.Friendship = newFriendship;

            await friendRequestRepository.UpdateAsync(friendshipRequest, cancellationToken);

            await friendshipRepository.InsertAsync(newFriendship, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success();
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(
                new Error("FriendRequest.Exception",
                    $"Failed to accept friend request: {ex.Message}"));
        }
    }

    public async Task<Result<string>> RejectFriendRequestAsync(Guid requestId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (requestId == Guid.Empty)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.InvalidId", "Invalid friend request ID"));
            }

            var friendship =
                await friendRequestRepository.GetByIdAsync(requestId, cancellationToken);
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

            friendship.Status = FriendRequestStatus.Rejected;

            await friendRequestRepository.UpdateAsync(friendship, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Friend request rejected successfully");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(
                new Error("FriendRequest.Exception",
                    $"Failed to reject friend request: {ex.Message}"));
        }
    }

    public async Task<Result<string>> WithdrawFriendRequestAsync(Guid requestId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (requestId == Guid.Empty)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.InvalidId", "Invalid friend request ID"));
            }

            var friendRequest =
                await friendRequestRepository.GetByIdAsync(requestId, cancellationToken);
            if (friendRequest == null)
            {
                return Result<string>.Failure(
                    new Error("FriendRequest.NotFound", "Friend request not found"));
            }

            if (friendRequest.Status != FriendRequestStatus.Pending)
            {
                string errorMessage = friendRequest.Status switch
                {
                    FriendRequestStatus.Accepted =>
                        "This friend request has already been accepted and cannot be withdrawn.",
                    FriendRequestStatus.Rejected =>
                        "This friend request has already been rejected and cannot be withdrawn.",
                    _ =>
                        $"This request cannot be withdrawn because it has a status of {friendRequest.Status} instead of Pending."
                };

                return Result<string>.Failure(
                    new Error(
                        $"FriendRequest.{friendRequest.Status}Status",
                        errorMessage
                    )
                );
            }

            friendRequest.Status = FriendRequestStatus.Withdrawn;

            await friendRequestRepository.UpdateAsync(friendRequest, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

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
            var currentUserId = userAccessor.GetCurrentUserId();

            var pendingRequests =
                await friendRequestRepository.GetPendingReceivedFriendRequestsAsync(currentUserId);

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
            var currentUserId = userAccessor.GetCurrentUserId();

            var pendingSentRequests =
                await friendRequestRepository.GetPendingSentFriendRequestsAsync(currentUserId);

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
                        CreatedAt = request.CreatedAt,
                        Status = request.Status,
                        TimeElapsed = Helpers.GetTimeElapsedString(request.CreatedAt)
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
}