using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;

namespace Kite.Application.Services;

public class RealTimeNotificationSender(INotificationHubContext notificationHubContext)
    : IRealTimeNotificationSender
{
    public async Task<Result<bool>> SendNotificationAsync(string userId,
        NotificationModel notification, CancellationToken cancellationToken)
    {
        try
        {
            var result = await notificationHubContext.SendToUserAsync(userId, "ReceiveNotification",
                notification, cancellationToken);

            if (!result.IsSuccess)
            {
                return Result<bool>.Failure($"Failed to send notification to user {userId}");
            }

            return Result<bool>.Success();
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(
                $"An error occurred while attempting to send notification to user {userId}: {ex.Message}");
        }
    }
}