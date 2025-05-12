using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kite.Infrastructure.Repositories;

public class FriendshipRepository : GenericRepository<Friendship, Guid>, IFriendshipRepository
{
    public FriendshipRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Friendship?> CheckIfFrienshipExists(
        string userIdOne, 
        string userIdTwo, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(f =>
                    (f.SenderId == userIdOne && f.ReceiverId == userIdTwo) ||
                    (f.SenderId == userIdTwo && f.ReceiverId == userIdOne),
                cancellationToken);
    }

    public async Task<IEnumerable<Friendship?>> GetPendingReceivedRequestsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.ReceiverId == userId && f.Status == FriendRequestStatus.Pending)
            .OrderByDescending(f => f.RequestReceivedTime)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Friendship?>> GetPendingSentRequestsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.SenderId == userId && f.Status == FriendRequestStatus.Pending)
            .OrderByDescending(f => f.RequestSentTime)
            .ToListAsync(cancellationToken);
    }
    //RequestReceivedTime and RequestSentTime should be fixed in service
    
}