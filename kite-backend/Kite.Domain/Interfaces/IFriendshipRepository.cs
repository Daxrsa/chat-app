using Kite.Domain.Entities;

namespace Kite.Domain.Interfaces;

public interface IFriendshipRepository : IGenericRepository<Friendship, Guid>
{
    Task<Friendship?> CheckIfFrienshipExists(string userIdOne, string userIdTwo,
        CancellationToken cancellationToken = default);
}