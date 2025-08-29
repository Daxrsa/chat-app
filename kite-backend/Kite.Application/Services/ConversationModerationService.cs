using Kite.Application.Interfaces;
using Kite.Domain.Common;

namespace Kite.Application.Services;

public class ConversationModerationService : IConversationModerationService
{
    public Task<Result<bool>> BanParticipantAsync(Guid conversationId, string userId, string? reason = null,
        DateTimeOffset? expiresAt = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> RevokeBanAsync(Guid conversationId, string userId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> KickParticipantAsync(Guid conversationId, string targetUserId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> MuteParticipantAsync(Guid conversationId, string targetUserId,
        DateTimeOffset? expiresAt = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UnmuteParticipantAsync(Guid conversationId, string targetUserId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}