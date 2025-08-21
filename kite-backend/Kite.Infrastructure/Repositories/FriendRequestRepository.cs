using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kite.Infrastructure.Repositories;

public class FriendRequestRepository : GenericRepository<FriendRequest, Guid>, IFriendRequestRepository
{
    public FriendRequestRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<FriendRequest>?> GetPendingReceivedFriendRequestsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.ReceiverId == userId && f.Status == FriendRequestStatus.Pending)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<FriendRequest>?> GetPendingSentFriendRequestsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.SenderId == userId && f.Status == FriendRequestStatus.Pending)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<FriendRequest>> GetAcceptedFriendRequestForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => (f.SenderId == userId || f.ReceiverId == userId) 
                        && f.Status == FriendRequestStatus.Accepted)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<FriendRequest?> GetFriendRequestBetweenUsersAsync(string currentUserId, string targetUserId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(fr => 
                (fr.SenderId == currentUserId && fr.ReceiverId == targetUserId) || 
                (fr.SenderId == targetUserId && fr.ReceiverId == currentUserId))
            .FirstOrDefaultAsync(cancellationToken);
    }
}