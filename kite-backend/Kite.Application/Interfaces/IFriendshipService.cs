using Kite.Application.Models;
using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

public interface IFriendshipService
{
    Task<Result<string>> SendFriendRequestAsync(string targetUserId);
    Task<Result<string>> AcceptFriendRequestAsync(Guid requestId);
    Task<Result<string>> RejectFriendRequestAsync(Guid requestId);
    Task<Result<string>> WithdrawFriendRequestAsync(Guid requestId);
    Task<Result<IEnumerable<FriendRequestModel>>> GetPendingReceivedRequestsAsync();
    Task<Result<IEnumerable<FriendRequestModel>>> GetPendingSentRequestsAsync();
    Task<Result<IEnumerable<UserModel>>> GetFriendsAsync();
}