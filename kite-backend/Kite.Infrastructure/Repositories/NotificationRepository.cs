using Kite.Domain.Entities;
using Kite.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kite.Infrastructure.Repositories;

public class NotificationRepository : GenericRepository<Notification, Guid>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(string userId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Where(n => n.ReceiverId == userId)
            .Include(n => n.Sender)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Notification?> GetNotificationByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(n => n.Sender)
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }
}