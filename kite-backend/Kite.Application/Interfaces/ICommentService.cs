using Kite.Application.Models;
using Kite.Domain.Common;
using Kite.Domain.Entities;

namespace Kite.Application.Interfaces;

public interface ICommentService
{
    Task<Result<CommentModel>> AddCommentAsync(CreateCommentRequest request, CancellationToken cancellationToken = default);
    Task<Result<CommentModel>> AddReplyAsync(Guid parentCommentId, string content, CancellationToken cancellationToken = default);
    Task<Result<CommentModel>> UpdateCommentAsync(Guid commentId, string content, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteCommentAsync(Guid commentId, CancellationToken cancellationToken = default);
}