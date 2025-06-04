using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class NotificationController(INotificationService notificationService) : BaseApiController
{
    [HttpGet("get-all-notifications")]
    public Task<Result<List<NotificationModel>>> GetAllNotifications(CancellationToken cancellationToken) =>
        notificationService.GetNotificationsForUserAsync(cancellationToken);

    [HttpPut("mark-as-read/{id}")]
    public Task<Result<bool>> MarkAsRead(Guid id, CancellationToken cancellationToken) =>
        notificationService.MarkNotificationAsReadAsync(id, cancellationToken);

    [HttpDelete("delete-notification/{id}")]
    public Task<Result<bool>> DeleteNotification(Guid id, CancellationToken cancellationToken) =>
        notificationService.DeleteNotificationAsync(id, cancellationToken);
}