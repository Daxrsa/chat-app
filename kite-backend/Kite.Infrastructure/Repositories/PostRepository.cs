using Kite.Domain.Entities;
using Kite.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kite.Infrastructure.Repositories;

public class PostRepository : GenericRepository<Post, Guid>, IPostRepository
{
    public PostRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Post>> GetPostsForUserAsync(string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.Files)
            .Include(x => x.MentionedUsers)
            .Include(x => x.Hashtags)
            .Include(x => x.User)
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}