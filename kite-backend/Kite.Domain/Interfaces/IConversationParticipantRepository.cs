using Kite.Domain.Entities;

namespace Kite.Domain.Interfaces;

public interface IConversationParticipantRepository : IGenericRepository<ConversationParticipant, Guid>
{
    
}