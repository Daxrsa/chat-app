using Kite.Application.Models;
using Kite.Application.Models.Post;
using Kite.Domain.Common;

namespace Kite.Application.Interfaces;

public interface IPostService
{
    Task<Result<PostModel>> CreatePostAsync(CreatePostRequest request, CancellationToken cancellationToken = default);
    Task<Result<PostModel>> GetSinglePostAsync(Guid postId, CancellationToken cancellationToken = default);
    Task<Result<PostModel>> UpdatePostAsync(Guid postId, UpdatePostRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeletePostAsync(Guid postId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetPostTotalReactionsAsync(Guid postId, CancellationToken cancellationToken = default);

    // // Feed and discovery
    // Task<Result<List<PostModel>>> GetUserFeedAsync(CancellationToken cancellationToken = default);
    // Task<Result<List<PostModel>>> GetPublicFeedAsync(CancellationToken cancellationToken = default);
    //
    // // Search and filtering
    // Task<Result<List<PostModel>>> SearchPostsAsync(string searchTerm, CancellationToken cancellationToken = default);
    // Task<Result<List<PostModel>>> GetPostsByHashtagAsync(string hashtag, CancellationToken cancellationToken = default);
    //
    // // Engagement features
    // Task<Result<bool>> IsPostLikedByUserAsync(Guid postId, CancellationToken cancellationToken = default);
    //
    // // Privacy and moderation
    // Task<Result<bool>> ReportPostAsync(Guid postId, string reason, CancellationToken cancellationToken = default);
    // Task<Result<bool>> HidePostAsync(Guid postId, CancellationToken cancellationToken = default);
    // Task<Result<bool>> UnhidePostAsync(Guid postId, CancellationToken cancellationToken = default);
    //
    // // Analytics (for user insights)
    // Task<Result<PostAnalyticsModel>> GetPostAnalyticsAsync(Guid postId, CancellationToken cancellationToken = default);
    // Task<Result<List<PostModel>>> GetTrendingPostsAsync(int limit = 10, CancellationToken cancellationToken = default);
}