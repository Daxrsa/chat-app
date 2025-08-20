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
    IFriendRequestRepository friendRequestRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IApplicationFileRepository applicationFileRepository,
    IFriendshipRepository friendshipRepository) : IFriendshipService
{
    public async Task<Result<string>> RemoveFriendAsync(string friendUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = userAccessor.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Result<string>.Failure(new Error("Auth.Unauthorized",
                    "User must be authenticated."));
            }

            if (string.IsNullOrEmpty(friendUserId))
            {
                return Result<string>.Failure(
                    new Error("FriendRemoval.InvalidTarget", "Friend user ID cannot be empty"));
            }

            var friendUser = await userManager.FindByIdAsync(friendUserId);
            if (friendUser == null)
            {
                return Result<string>.Failure(
                    new Error("FriendRemoval.UserNotFound", "The specified friend does not exist"));
            }

            var existingFriendship =
                await friendshipRepository.CheckIfFrienshipExists(currentUserId, friendUserId,
                    cancellationToken);
            if (existingFriendship == null)
            {
                return Result<string>.Failure(
                    new Error("FriendRemoval.NotFriends", "You are not friends with this user"));
            }

            await friendshipRepository.DeleteAsync(existingFriendship, cancellationToken);

            var friendRequest = await friendRequestRepository.GetFriendRequestBetweenUsersAsync(
                currentUserId, friendUserId, cancellationToken);

            if (friendRequest != null)
            {
                friendRequest.Status = FriendRequestStatus.Rejected;
                friendRequest.UpdatedAt = DateTime.UtcNow;
                await friendRequestRepository.UpdateAsync(friendRequest, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(
                "Friend has been successfully removed from your friend list.");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(
                new Error("FriendRemoval.Exception",
                    $"Failed to remove friend: {ex.Message}"));
        }
    }

    public async Task<Result<IEnumerable<UserModel>>> GetFriendsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = userAccessor.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Result<IEnumerable<UserModel>>.Failure(new Error("Auth.Unauthorized",
                    "User must be authenticated."));
            }

            var friendships =
                await friendRequestRepository.GetAcceptedFriendRequestForUserAsync(currentUserId,
                    cancellationToken);

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
                var authorProfilePicture =
                    await applicationFileRepository.GetLatestUserFileByTypeAsync(friendId,
                        FileType.Post, cancellationToken);

                var friendUser = await userRepository.GetByIdAsync(friendId, cancellationToken);
                if (friendUser != null)
                {
                    friendModels.Add(new UserModel
                    {
                        Id = friendUser.Id,
                        UserName = friendUser.UserName ?? string.Empty,
                        FirstName = friendUser.FirstName,
                        LastName = friendUser.LastName,
                        Email = friendUser.Email ?? string.Empty,
                        ProfilePicture = authorProfilePicture?.FilePath ?? string.Empty,
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

    public async Task<Result<IEnumerable<UserModel>>> GetMutualFriendsAsync(string targetUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = userAccessor.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Result<IEnumerable<UserModel>>.Failure(new Error("Auth.Unauthorized",
                    "User must be authenticated."));
            }

            if (string.IsNullOrEmpty(targetUserId))
            {
                return Result<IEnumerable<UserModel>>.Failure(
                    new Error("MutualFriends.InvalidTarget", "Target user ID cannot be empty"));
            }

            var targetUser = await userManager.FindByIdAsync(targetUserId);
            if (targetUser == null)
            {
                return Result<IEnumerable<UserModel>>.Failure(
                    new Error("MutualFriends.UserNotFound",
                        "The specified target user does not exist"));
            }

            var currentUserFriendships =
                await friendRequestRepository.GetAcceptedFriendRequestForUserAsync(currentUserId,
                    cancellationToken);

            var currentUserFriendIds = new HashSet<string>();
            foreach (var friendship in currentUserFriendships)
            {
                if (friendship.SenderId == currentUserId)
                {
                    currentUserFriendIds.Add(friendship.ReceiverId);
                }
                else if (friendship.ReceiverId == currentUserId)
                {
                    currentUserFriendIds.Add(friendship.SenderId);
                }
            }

            var targetUserFriendships =
                await friendRequestRepository.GetAcceptedFriendRequestForUserAsync(targetUserId,
                    cancellationToken);

            var targetUserFriendIds = new HashSet<string>();
            foreach (var friendship in targetUserFriendships)
            {
                if (friendship.SenderId == targetUserId)
                {
                    targetUserFriendIds.Add(friendship.ReceiverId);
                }
                else if (friendship.ReceiverId == targetUserId)
                {
                    targetUserFriendIds.Add(friendship.SenderId);
                }
            }

            var mutualFriendIds = currentUserFriendIds.Intersect(targetUserFriendIds);

            var mutualFriendModels = new List<UserModel>();
            foreach (var friendId in mutualFriendIds)
            {
                var friendProfilePicture =
                    await applicationFileRepository.GetLatestUserFileByTypeAsync(friendId,
                        FileType.ProfilePicture, cancellationToken);

                var friendUser = await userRepository.GetByIdAsync(friendId, cancellationToken);
                if (friendUser != null)
                {
                    mutualFriendModels.Add(new UserModel
                    {
                        Id = friendUser.Id,
                        UserName = friendUser.UserName ?? string.Empty,
                        FirstName = friendUser.FirstName,
                        LastName = friendUser.LastName,
                        Email = friendUser.Email ?? string.Empty,
                        ProfilePicture = friendProfilePicture?.FilePath ?? string.Empty,
                    });
                }
            }

            return Result<IEnumerable<UserModel>>.Success(mutualFriendModels);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<UserModel>>.Failure(
                new Error("MutualFriends.Exception",
                    $"Failed to retrieve mutual friends: {ex.Message}"));
        }
    }
}