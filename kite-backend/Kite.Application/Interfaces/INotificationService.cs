using Kite.Application.Models;
using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

public interface INotificationService
{
    Task<Result<NotificationModel>> CreateNotificationAsync(NotificationModel notificationModel, CancellationToken cancellationToken = default);
}