using Kite.Domain.Entities;

namespace Kite.Domain.Interfaces;

public interface INotificationRepository : IGenericRepository<Notification, Guid>
{
    Task<Notification?> GetExistingNotificationAsync(Notification request,
        CancellationToken cancellationToken);
    Task<IEnumerable<Notification>> GetNotificationsForUserAsync(string userId,
        CancellationToken cancellationToken);
    Task<Notification> GetNotificationByIdAsync(Guid id,
        CancellationToken cancellationToken = default);
}