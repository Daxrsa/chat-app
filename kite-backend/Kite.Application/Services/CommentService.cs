using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Kite.Application.Services;

public class CommentService(IUserAccessor userAccessor, UserManager<ApplicationUser> userManager) : ICommentService
{
    public async Task<Result<CommentModel>> AddCommentAsync(Guid postId, string content, CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<CommentModel>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated to create posts"));
        }

        var currentUser = await userManager.FindByIdAsync(currentUserId);
        if (currentUser == null)
        {
            return Result<CommentModel>.Failure(new Error("User.NotFound", "User not found"));
        }
    }

    public async Task<Result<CommentModel>> AddReplyAsync(Guid parentCommentId, string content,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<CommentModel>> UpdateCommentAsync(Guid commentId, string content,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<bool>> DeleteCommentAsync(Guid commentId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<CommentModel>>> GetPostCommentsAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}