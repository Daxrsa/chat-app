using Kite.Domain.Entities;
using Kite.Domain.Interfaces;

namespace Kite.Infrastructure.Repositories;

public class ConversationBanRepository : GenericRepository<ConversationBan, Guid>, IConversationBanRepository
{
    public ConversationBanRepository(AppDbContext context) : base(context)
    {
    }
}