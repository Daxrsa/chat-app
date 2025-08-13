using Kite.Domain.Entities;
using Kite.Domain.Interfaces;

namespace Kite.Infrastructure.Repositories;

public class MessageRepository : GenericRepository<Message, Guid>, IMessageRepository
{
    public MessageRepository(AppDbContext context) : base(context)
    {
    }
}