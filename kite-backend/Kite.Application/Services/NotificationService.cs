using AutoMapper;
using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Application.Utilities;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;

namespace Kite.Application.Services;

public class NotificationService(
    INotificationRepository notificationRepository,
    IMapper mapper,
    IRealTimeNotificationSender realTimeNotificationSender,
    IUserAccessor userAccessor,
    INotificationHubContext signalRHubContext,
    IUnitOfWork unitOfWork) : INotificationService
{
    public async Task<Result<NotificationModel>> CreateNotificationAsync(NotificationModel request,
        CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = userAccessor.GetCurrentUserId();
            var notification = mapper.Map<Notification>(request);
            var response = new NotificationModel
            {
                Message = notification.Message,
                ReceiverId = notification.ReceiverId,
                SenderId = currentUserId,
                Type = notification.Type,
                CreatedAt = DateTimeOffset.UtcNow,
                TimeElapsed = Helpers.GetTimeElapsedString(request.CreatedAt),
                IsRead = notification.IsRead,
                Id = notification.Id
            };

            await realTimeNotificationSender.SendNotificationAsync(request.ReceiverId, response,
                cancellationToken);
            await notificationRepository.InsertAsync(notification, cancellationToken);
            await  unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<NotificationModel>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<NotificationModel>.Failure(
                $"An unexpected Error occured during notification creation: {ex.Message}");
        }
    }

    public async Task<Result<List<NotificationModel>>> GetNotificationsForUserAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = userAccessor.GetCurrentUserId();
            var notifications =
                await notificationRepository.GetNotificationsForUserAsync(currentUserId,
                    cancellationToken);
            var response = mapper.Map<List<NotificationModel>>(notifications);
            
            await signalRHubContext.SendToUserAsync(currentUserId, "NotificationReceived", response, cancellationToken);
            
            return Result<List<NotificationModel>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<List<NotificationModel>>.Failure(
                $"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<bool>> MarkNotificationAsReadAsync(Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = userAccessor.GetCurrentUserId();
            var notification =
                await notificationRepository.GetNotificationByIdAsync(id, cancellationToken);

            if (notification == null)
            {
                return Result<bool>.Failure($"Notification with ID {id} not found");
            }

            if (notification.ReceiverId != currentUserId)
            {
                return Result<bool>.Failure(
                    "You are not authorized to mark this notification as read");
            }

            notification.IsRead = true;
            await notificationRepository.UpdateAsync(notification, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            await signalRHubContext.SendToUserAsync(currentUserId, "NotificationMarkedAsRead", notification.Id, cancellationToken);

            return Result<bool>.Success();
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteNotificationAsync(Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = userAccessor.GetCurrentUserId();
            var notification =
                await notificationRepository.GetNotificationByIdAsync(id, cancellationToken);

            if (notification == null)
            {
                return Result<bool>.Failure($"Notification with ID {id} not found");
            }

            if (notification.ReceiverId != currentUserId)
            {
                return Result<bool>.Failure("You are not authorized to delete this notification");
            }

            await notificationRepository.DeleteAsync(notification, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            await signalRHubContext.SendToUserAsync(currentUserId, "NotificationDeleted", notification.Id, cancellationToken);

            return Result<bool>.Success();
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}