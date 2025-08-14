using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;

namespace Kite.Application.Services;

public class ConversationParticipantService : IConversationParticipantService
{
    public Task<Result<ConversationParticipantModel>> AddParticipantAsync(Guid conversationId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> RemoveParticipantAsync(Guid conversationId, string userId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<ConversationParticipantModel>>> GetParticipantsAsync(
        Guid conversationId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UpdateLastReadTimestampAsync(Guid conversationId, string userId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}