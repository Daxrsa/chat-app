using Kite.Domain.Entities;

namespace Kite.Domain.Interfaces;

public interface IPostRepository : IGenericRepository<Post, Guid>
{
    Task<IEnumerable<Post>> GetPostsForUserAsync(string userId,
        CancellationToken cancellationToken = default);
}