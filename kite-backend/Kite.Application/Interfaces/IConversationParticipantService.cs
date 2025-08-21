using Kite.Application.Models;
using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

public interface IConversationParticipantService
{
    Task<Result<ConversationParticipantModel>> GetSingleParticipantAsync(
        Guid participantId,
        CancellationToken cancellationToken = default);

    Task<Result<ConversationParticipantModel>> AddParticipantAsync(Guid conversationId,
        string userId, CancellationToken cancellationToken = default);

    Task<Result<bool>> RemoveParticipantAsync(Guid conversationId, string userId,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<ConversationParticipantModel>>> GetNonParticipantsAsync(
        Guid conversationId, CancellationToken cancellationToken = default);

    Task<Result<List<ConversationParticipantModel>>> GetParticipantsAsync(Guid conversationId,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> PromoteToModeratorAsync(Guid conversationId, string userId,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DemoteModeratorAsync(Guid conversationId, string userId,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> TransferOwnershipAsync(Guid conversationId, string newOwnerUserId,
        CancellationToken cancellationToken = default);
}