using Kite.Domain.Entities;

namespace Kite.Domain.Interfaces;

public interface IConversationRepository : IGenericRepository<Conversation, Guid>
{
    Task<Conversation?> FindByParticipantsAsync(List<string> participantIds,
        CancellationToken cancellationToken = default);

    Task<List<Conversation>> GetConversationsByUserIdAsync(string userId,
        CancellationToken cancellationToken = default);
}