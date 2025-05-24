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
    
    public async Task<Friendship?> CheckIfFrienshipExists(string userIdOne, string userIdTwo,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.FriendRequest)
            .Where(f => f.FriendRequest.Status == FriendRequestStatus.Accepted)
            .Where(f => (f.FriendRequest.SenderId == userIdOne && f.FriendRequest.ReceiverId == userIdTwo) ||
                        (f.FriendRequest.SenderId == userIdTwo && f.FriendRequest.ReceiverId == userIdOne))
            .FirstOrDefaultAsync(cancellationToken);
    }
}