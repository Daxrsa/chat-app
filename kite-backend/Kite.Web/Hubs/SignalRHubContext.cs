using Kite.Application.Interfaces;
using Kite.Domain.Common;
using Kite.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Kite.Web.Hubs;

// Small adapter that wraps SignalR to send a single notification to a specific user.
public class SignalRHubContext(
    IHubContext<NotificationHub> hubContext) : INotificationHubContext
{
    public async Task<Result<bool>> SendToUserAsync<T>(string userId, string method, T arg,
        CancellationToken cancellationToken)
    {
        try
        {
            await hubContext.Clients.User(userId).SendAsync(method, arg, cancellationToken);
            return Result<bool>.Success();
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(
                $"Failed sending message via SignalR to user {userId} using method {method}: {ex.Message}");
        }
    }
}