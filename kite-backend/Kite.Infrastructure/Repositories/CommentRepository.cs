using Kite.Domain.Entities;
using Kite.Domain.Interfaces;

namespace Kite.Infrastructure.Repositories;

public class CommentRepository : GenericRepository<Comment, Guid>, ICommentRepository
{
    public CommentRepository(AppDbContext context) : base(context)
    {
    }
}