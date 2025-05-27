using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Kite.Infrastructure.Hubs;

public sealed class NotificationHub(
    IUserAccessor userAccessor,
    ILogger<NotificationHub> logger) : Hub
{
    public async Task SendNotification(string userId, string message)
    {
        try
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", new { Message = message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error sending notification to user {userId}: {ex.Message}");
        }
    }

    public override async Task<Result<bool>> OnConnectedAsync()
    {
        try
        {
            var currentUser = userAccessor.GetCurrentUserId();

            if (string.IsNullOrEmpty(currentUser))
            {
                return Result<bool>.Failure("User connection failed: invalid user ID.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, currentUser);
            logger.LogInformation(
                "User {UserId} successfully connected with ConnectionId {ConnectionId}",
                currentUser, Context.ConnectionId);

            await base.OnConnectedAsync();
            return Result<bool>.Success();
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(
                $"An unexpected error occurred while attempting to connect user: {ex.Message}");
        }
    }

    public override async Task<Result<bool>> OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var currentUser = userAccessor.GetCurrentUserId();

            if (string.IsNullOrEmpty(currentUser))
            {
                return Result<bool>.Failure("User disconnection failed: invalid user ID.");
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentUser);
            logger.LogInformation(
                "User {UserId} successfully disconnected and removed from group with ConnectionId {ConnectionId}",
                currentUser, Context.ConnectionId);

            if (exception != null)
            {
                return Result<bool>.Failure($"Disconnection error: {exception.Message}");
            }

            await base.OnDisconnectedAsync(exception);
            return Result<bool>.Success();
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(
                $"An unexpected error occurred while attempting to disconnect user: {ex.Message}");
        }
    }
}