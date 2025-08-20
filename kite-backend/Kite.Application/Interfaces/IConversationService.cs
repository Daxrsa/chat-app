using Kite.Application.Models;
using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

public interface IConversationService
{
    Task<Result<IEnumerable<MessageModel>>> GetConversationMessagesAsync(Guid conversationId,
        CancellationToken cancellationToken = default);
    Task<Result<ConversationModel>> CreateConversationAsync(List<string> participantIds,
        CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteConversationAsync(Guid conversationId,
        CancellationToken cancellationToken = default); //only an owner can delete a conversation
}