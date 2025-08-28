using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

// Small adapter that wraps SignalR to send a single notification to a specific user.
public interface INotificationHubContext
{
    Task<Result<bool>> SendToUserAsync<T>(string userId, string method, T arg, CancellationToken cancellationToken);
}