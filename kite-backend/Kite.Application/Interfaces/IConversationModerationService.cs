using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

public interface IConversationModerationService
{
    Task<Result<bool>> BanParticipantAsync(
        Guid conversationId,
        string userId,
        string? reason = null,
        DateTimeOffset? expiresAt = null,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> RevokeBanAsync(
        Guid conversationId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> KickParticipantAsync(
        Guid conversationId,
        string targetUserId,
        CancellationToken cancellationToken = default);
    
    Task<Result<bool>> MuteParticipantAsync(
        Guid conversationId,
        string targetUserId,
        DateTimeOffset? expiresAt = null,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> UnmuteParticipantAsync(
        Guid conversationId,
        string targetUserId,
        CancellationToken cancellationToken = default);
}