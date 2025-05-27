using Kite.Application.Models;
using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

public interface IRealTimeNotificationSender
{
    Task<Result<bool>> SendNotificationAsync(string userId, NotificationModel notification, CancellationToken cancellationToken);
}