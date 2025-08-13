using Kite.Application.Models;
using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

public interface IConversationParticipantService
{
    Task<Result<ConversationParticipantModel>> AddParticipantAsync(Guid conversationId, string userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveParticipantAsync(Guid conversationId, string userId, CancellationToken cancellationToken = default);
    Task<Result<List<ConversationParticipantModel>>> GetParticipantsAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateLastReadTimestampAsync(Guid conversationId, string userId, CancellationToken cancellationToken = default);
}