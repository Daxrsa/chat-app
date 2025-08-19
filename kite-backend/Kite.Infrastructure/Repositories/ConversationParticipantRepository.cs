using Kite.Domain.Entities;
using Kite.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kite.Infrastructure.Repositories;

public class ConversationParticipantRepository : GenericRepository<ConversationParticipant, Guid>, IConversationParticipantRepository
{
    public ConversationParticipantRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task<List<ApplicationUser>> GetNonParticipantsAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var participantUserIds = await _dbSet
            .Where(p => p.ConversationId == conversationId)
            .Select(p => p.UserId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return await _context.Set<ApplicationUser>()
            .Where(u => !participantUserIds.Contains(u.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<List<ApplicationUser>> GetConversationParticipantsAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.ConversationId == conversationId)
            .Select(p => p.User)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}