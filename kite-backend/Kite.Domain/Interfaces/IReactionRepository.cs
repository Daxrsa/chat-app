using Kite.Domain.Entities;
using Kite.Domain.Enums;

namespace Kite.Domain.Interfaces;

public interface IReactionRepository : IGenericRepository<Reaction, Guid>
{
    Task<Reaction?> GetByEntityAndUserAsync(Guid entityId, EntityType
        entityType, string userId, CancellationToken cancellationToken = default);
    Task<int> GetTotalReactionsByEntityAsync(Guid entityId, EntityType entityType,
        CancellationToken cancellationToken = default);
}