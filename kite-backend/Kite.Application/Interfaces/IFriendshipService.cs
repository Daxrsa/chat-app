using Kite.Application.Models;
using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

public interface IFriendshipService
{
    Task<Result<string>> RemoveFriendAsync(string friendUserId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UserModel>>> GetFriendsAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UserModel>>> GetMutualFriendsAsync(string targetUserId, CancellationToken cancellationToken = default);
}