using Kite.Domain.Common;
using Kite.Domain.Enums;

namespace Kite.Application.Interfaces;

public interface IReactionService
{
    Task<Result<bool>> AddOrUpdateReactionAsync(EntityType entityType, Guid entityId,
        ReactionType reactionType,
        CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveReaction(Guid reactionId,
        CancellationToken cancellationToken = default);
}