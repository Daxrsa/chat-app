using Kite.Domain.Entities;

namespace Kite.Domain.Interfaces;

public interface IFriendshipRepository : IGenericRepository<Friendship, Guid>
{
    Task<Friendship?> CheckIfFrienshipExists(string userIdOne, string userIdTwo, CancellationToken cancellationToken = default);
    Task<IEnumerable<Friendship?>> GetPendingReceivedRequestsAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Friendship?>> GetPendingSentRequestsAsync(string userId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Friendship>> GetAcceptedFriendshipsForUserAsync(string userId, CancellationToken cancellationToken = default);
}