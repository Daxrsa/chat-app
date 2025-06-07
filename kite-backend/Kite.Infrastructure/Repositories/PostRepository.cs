using Kite.Domain.Entities;
using Kite.Domain.Interfaces;

namespace Kite.Infrastructure.Repositories;

public class PostRepository : GenericRepository<Post, Guid>, IPostRepository
{
    public PostRepository(AppDbContext context) : base(context)
    {
    }
}