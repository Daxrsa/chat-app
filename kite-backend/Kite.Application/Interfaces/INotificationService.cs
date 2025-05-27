using Kite.Application.Models;
using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

public interface INotificationService
{
    Task<Result<NotificationModel>> CreateNotificationAsync(NotificationModel request,
        CancellationToken cancellationToken);
    Task<Result<NotificationModel>> GetNotificationByUserIdAsync(string userId, Guid notificationId,
        CancellationToken cancellationToken);
    Task<Result<List<NotificationModel>>> GetNotificationsForUserAsync(string userId,
        CancellationToken cancellationToken);
    Task<Result<bool>> MarkNotificationAsReadAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteNotificationAsync(Guid id, CancellationToken cancellationToken);
}