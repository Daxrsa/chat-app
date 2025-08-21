using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;

namespace Kite.Application.Interfaces;

public interface IUserService
{
    Task<Result<IEnumerable<ConversationModel>>> GetUserConversationsAsync(
        CancellationToken cancellationToken = default);
    Task<Result<List<PostModel>>> GetPostsForCurrentUserAsync(
        CancellationToken cancellationToken = default);
    Task<Result<List<PostModel>>> GetPostsForUserAsync(string userId,
        CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ConversationModel>>> GetMutualConversationsAsync(
        string targetUserId,
        CancellationToken cancellationToken = default);
}