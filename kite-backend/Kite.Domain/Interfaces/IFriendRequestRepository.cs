using Kite.Domain.Entities;

namespace Kite.Domain.Interfaces;

public interface IFriendRequestRepository : IGenericRepository<FriendRequest, Guid>
{
    Task<IEnumerable<FriendRequest?>> GetPendingReceivedFriendRequestsAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FriendRequest?>> GetPendingSentFriendRequestsAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FriendRequest>> GetAcceptedFriendRequestForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<FriendRequest?> GetFriendRequestBetweenUsersAsync(string currentUserId,
        string targetUserId, CancellationToken cancellationToken = default);
}