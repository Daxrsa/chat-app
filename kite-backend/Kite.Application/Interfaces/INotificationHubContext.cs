using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

public interface INotificationHubContext
{
    Task<Result<bool>> SendToUserAsync<T>(string userId, string method, T arg, CancellationToken cancellationToken);
}