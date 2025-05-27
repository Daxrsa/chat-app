using AutoMapper;
using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;

namespace Kite.Application.Services;

public class NotificationService(
    INotificationRepository notificationRepository,
    IMapper mapper,
    IRealTimeNotificationSender realTimeNotificationSender,
    IUnitOfWork unitOfWork) : INotificationService
{
    public async Task<Result<NotificationModel>> CreateNotificationAsync(NotificationModel request, CancellationToken cancellationToken)
    {
        try
        {
            var notification = mapper.Map<Notification>(request);
            notification.CreatedAt = DateTimeOffset.UtcNow;
            var existingNotification = await notificationRepository.GetExistingNotificationAsync(notification, cancellationToken);
            if (existingNotification is not null)
            {
                existingNotification.Message = request.Message;
                existingNotification.IsRead = false;
                existingNotification.Type = NotificationType.Message;
                await notificationRepository.UpdateAsync(existingNotification, cancellationToken);
            }
           
            notification.IsRead = false;
            await notificationRepository.InsertAsync(notification, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new NotificationModel
            {
                Message = existingNotification?.Message ?? notification.Message,
                ReceiverId = notification.ReceiverId,
                SenderId = notification.SenderId,
                Type = notification.Type,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead,
                Id = notification.Id
            };

            await realTimeNotificationSender.SendNotificationAsync(request.ReceiverId, response, cancellationToken);
            return Result<NotificationModel>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<NotificationModel>.Failure($"An unexpected Error occured during notification creation: {ex.Message}");
        }
    }
    
    public async Task<Result<NotificationModel>> GetNotificationByUserIdAsync(string userId, Guid notificationId, CancellationToken cancellationToken)
    {
        try
        {
            var notification = await notificationRepository.GetNotificationByIdAsync(notificationId, cancellationToken);
            if (notification == null || notification.ReceiverId != userId)
            {
                return Result<NotificationModel>.Failure($"Notification with ID: {notificationId} not found");
            }

            var response = mapper.Map<NotificationModel>(notification);
            return Result<NotificationModel>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<NotificationModel>.Failure($"Error retrieving notification: {ex.Message}");
        }
    }
    
    public async Task<Result<List<NotificationModel>>> GetNotificationsForUserAsync(string userId, CancellationToken cancellationToken)
    {
        try
        {
            var notifications = await notificationRepository.GetNotificationsForUserAsync(userId, cancellationToken);
            var response = mapper.Map<List<NotificationModel>>(notifications);

            return Result<List<NotificationModel>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<List<NotificationModel>>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
    
    public async Task<Result<bool>> MarkNotificationAsReadAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var notification = await notificationRepository.GetNotificationByIdAsync(id, cancellationToken);
            if (notification == null)
            {
                return Result<bool>.Failure($"Notification with ID {id} not found");
            }

            notification.IsRead = true;
            await notificationRepository.UpdateAsync(notification, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success();
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
    
    public async Task<Result<bool>> DeleteNotificationAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var notification = await notificationRepository.GetNotificationByIdAsync(id, cancellationToken);

            if (notification == null)
            {
                return Result<bool>.Failure($"Notification with ID {id} not found");
            }

            await notificationRepository.DeleteAsync(notification, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success();
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}