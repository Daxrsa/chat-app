using Kite.Domain.Entities;

namespace Kite.Domain.Interfaces;

public interface IConversationParticipantRepository : IGenericRepository<ConversationParticipant, Guid>
{
    Task<List<ApplicationUser>> GetNonParticipantsAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default);

    Task<List<ApplicationUser>> GetConversationParticipantsAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Conversation>?> GetMutualConversationsAsync(
        string userOneId,
        string userTwoId,
        CancellationToken cancellationToken = default);
}