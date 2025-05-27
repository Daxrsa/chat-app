using Kite.Application.Models;
using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

public interface IFriendRequestService
{
    Task<Result<string>> SendFriendRequestAsync(string targetUserId, CancellationToken cancellationToken = default);
    Task<Result<string>> AcceptFriendRequestAsync(Guid requestId,
        CancellationToken cancellationToken = default);
    Task<Result<string>> RejectFriendRequestAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<Result<string>> WithdrawFriendRequestAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<FriendRequestModel>>> GetPendingReceivedRequestsAsync();
    Task<Result<IEnumerable<FriendRequestModel>>> GetPendingSentRequestsAsync();
}