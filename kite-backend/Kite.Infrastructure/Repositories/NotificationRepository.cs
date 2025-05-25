using Kite.Domain.Entities;
using Kite.Domain.Interfaces;

namespace Kite.Infrastructure.Repositories;

public class NotificationRepository : GenericRepository<Notification, Guid>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context)
    {
    }
}