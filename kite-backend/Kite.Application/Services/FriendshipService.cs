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
    IFriendshipRepository friendshipRepository) : IFriendshipService
{
    public async Task<Result<string>> RemoveFriendAsync(string friendUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = userAccessor.GetCurrentUserId();

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
                await friendshipRepository.CheckIfFrienshipExists(currentUserId, friendUserId, cancellationToken);
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
    
    public async Task<Result<IEnumerable<UserModel>>> GetFriendsAsync()
    {
        try
        {
            var currentUserId = userAccessor.GetCurrentUserId();
            var friendships =
                await friendRequestRepository.GetAcceptedFriendRequestForUserAsync(currentUserId);

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
                        Email = friendUser.Email
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