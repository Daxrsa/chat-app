using Kite.Domain.Entities;

namespace Kite.Domain.Interfaces;

public interface ICommentRepository : IGenericRepository<Comment, Guid>
{
    
}