using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Kite.Application.Interfaces;

public interface ICommentService
{
    Task<Result<CommentModel>> AddCommentAsync(CreateCommentRequest request, CancellationToken cancellationToken = default);
    Task<Result<CommentModel>> AddReplyAsync(CreateCommentRequest request, CancellationToken cancellationToken = default);
    Task<Result<CommentModel>> UpdateCommentAsync(Guid commentId, string content, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteCommentAsync(Guid commentId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetCommentTotalReactionsAsync(Guid postId,
        CancellationToken cancellationToken = default);
}