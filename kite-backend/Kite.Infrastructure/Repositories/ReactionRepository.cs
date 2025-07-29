using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kite.Infrastructure.Repositories;

public class ReactionRepository : GenericRepository<Reaction, Guid>, IReactionRepository
{
    public ReactionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Reaction?> GetByEntityAndUserAsync(Guid entityId, EntityType entityType,
        string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.EntityId == entityId &&
                                                     r.EntityType == entityType &&
                                                     r.UserId == userId, cancellationToken);
    }
}