using Kite.Application.Interfaces;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;

namespace Kite.Application.Services;

public class ReactionService(
    IReactionRepository reactionRepository,
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor)
    : IReactionService
{
    public async Task<Result<bool>> AddOrUpdateReactionAsync(EntityType entityType, Guid entityId,
        ReactionType reactionType,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<bool>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated to make reactions"));
        }
        var existingReaction =
            await reactionRepository.GetByEntityAndUserAsync(entityId, entityType, currentUserId,
                cancellationToken);

        if (existingReaction != null)
        {
            existingReaction.ReactionType = reactionType;
            await reactionRepository.UpdateAsync(existingReaction, cancellationToken);
        }
        else
        {
            var newReaction = new Reaction
            {
                EntityId = entityId,
                EntityType = entityType,
                ReactionType = reactionType,
                UserId = currentUserId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await reactionRepository.InsertAsync(newReaction, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success();
    }

    public async Task<Result<bool>> RemoveReaction(Guid reactionId, CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<bool>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated to remove reactions"));
        }
        
        var existingReaction = await reactionRepository.GetByIdAsync(reactionId, cancellationToken);
        if (existingReaction == null)
        {
            return Result<bool>.Failure(new Error("Reaction.NotFound", "Reaction not found"));
        }
        
        if (existingReaction.UserId != currentUserId)
        {
            return Result<bool>.Failure(new Error("Auth.Forbidden", 
                "You can only remove your own reactions"));
        }
        
        await reactionRepository.DeleteAsync(existingReaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success();
    }
}