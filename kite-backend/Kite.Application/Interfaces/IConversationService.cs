using Kite.Application.Models;
using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

public interface IConversationService
{
    Task<Result<IEnumerable<ConversationModel>>> GetUserConversationsAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<MessageModel>>> GetConversationMessagesAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task<Result<ConversationModel>> CreateConversationAsync(List<string> participantIds, CancellationToken cancellationToken = default);
    Task<Result<bool>> MarkConversationAsReadAsync(Guid conversationId, CancellationToken cancellationToken = default);
}