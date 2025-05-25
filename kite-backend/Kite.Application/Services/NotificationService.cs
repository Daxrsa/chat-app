using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Kite.Domain.Interfaces;

namespace Kite.Application.Services;

public class NotificationService(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork) : INotificationService
{
    public async Task<Result<NotificationModel>> CreateNotificationAsync(
        NotificationModel notificationModel,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(notificationModel.ReceiverId))
                return Result<NotificationModel>.Failure("Receiver ID cannot be empty");

            if (string.IsNullOrEmpty(notificationModel.Message))
                return Result<NotificationModel>.Failure("Notification message cannot be empty");

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientId = notificationModel.ReceiverId,
                Title = notificationModel.Title,
                Message = notificationModel.Message,
                Type = notificationModel.Type,
                IsRead = notificationModel.IsRead,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await notificationRepository.InsertAsync(notification, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // _ = SendRealTimeNotificationAsync(notificationModel.ReceiverId, notification);

            return Result<NotificationModel>.Success();
        }
        catch (Exception ex)
        {
            return Result<NotificationModel>.Failure($"Error creating notification: {ex.Message}");
        }
    }
}