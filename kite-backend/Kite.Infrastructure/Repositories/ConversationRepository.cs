using Kite.Domain.Entities;
using Kite.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kite.Infrastructure.Repositories;

public class ConversationRepository : GenericRepository<Conversation, Guid>, IConversationRepository
{
    public ConversationRepository(AppDbContext context) : base(context)
    {
    }

    public Task<Conversation?> FindByParticipantsAsync(List<string> participantIds, CancellationToken cancellationToken = default)
    {
        var participantCount = participantIds.Count;
        return _dbSet
            .Include(c => c.Participants)
            .Where(c => c.Participants.Count == participantCount && 
                        c.Participants.All(p => participantIds.Contains(p.UserId)))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<Conversation>> GetConversationsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Include(c => c.Participants)
            .ThenInclude(p => p.User)
            .Include(c => c.Messages)
            .Where(c => c.Participants.Any(p => p.UserId == userId))
            .OrderByDescending(c => c.Messages.Any() 
                ? c.Messages.Max(m => m.SentAt) 
                : c.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}