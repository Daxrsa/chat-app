using Kite.Domain.Entities;
using Kite.Domain.Interfaces;

namespace Kite.Infrastructure.Repositories;

public class ConversationRepository : GenericRepository<Conversation, Guid>, IConversationRepository
{
    public ConversationRepository(AppDbContext context) : base(context)
    {
    }
}