using Kite.Domain.Entities;

namespace Kite.Domain.Interfaces;

public interface IMessageRepository : IGenericRepository<Message, Guid>
{
    
}