using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Application.Utilities;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kite.Application.Services;

public class CommentService(
    IUserAccessor userAccessor,
    UserManager<ApplicationUser> userManager,
    IFileUploaderService fileUploaderService,
    ICommentRepository commentRepository,
    IApplicationFileRepository applicationFileRepository,
    IUnitOfWork unitOfWork) : ICommentService
{
    public async Task<Result<CommentModel>> AddCommentAsync(CreateCommentRequest request,
        CancellationToken cancellationToken = default)
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

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            UserId = currentUserId,
            Content = request.Content,
            CreatedAt = DateTimeOffset.UtcNow,
            PostId = request.PostId,
            Files = []
        };

        if (request.Files != null && request.Files.Any())
        {
            var uploadResult = await fileUploaderService.UploadFilesAsync(request.Files,
                FileType.Comment, cancellationToken);

            if (uploadResult.IsSuccess && uploadResult.Value?.SuccessfulUploads != null)
            {
                comment.Files = uploadResult.Value.SuccessfulUploads.Select(fileResult =>
                    new ApplicationFile
                    {
                        Id = Guid.NewGuid(),
                        Filename = fileResult.OriginalFileName,
                        Extension = Path.GetExtension(fileResult.OriginalFileName),
                        Size = fileResult.FileSize,
                        FilePath = fileResult.FilePath,
                        Type = fileResult.Type,
                        UserId = currentUserId,
                        UploadedAt = fileResult.UploadedAt
                    }).ToList();
            }
        }

        var authorProfilePicture =
            await applicationFileRepository.GetLatestUserFileByTypeAsync(currentUserId,
                FileType.Post, cancellationToken);

        var commentModel = new CommentModel
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UserId = comment.UserId,
            AuthorFirstName = currentUser.FirstName,
            AuthorLastName = currentUser.LastName,
            AuthorUserName = currentUser.UserName ?? string.Empty,
            AuthorProfilePicture = authorProfilePicture?.FilePath,
            TimeElapsed = Helpers.GetTimeElapsedString(DateTimeOffset.UtcNow),
            Files = comment.Files?.Select(file => new AttachedFileModel
            {
                Id = file.Id,
                FileName = file.Filename,
                Extension = file.Extension,
                Size = file.Size,
                FilePath = file.FilePath,
                Type = file.Type,
                UserId = file.UserId,
                UploadedAt = file.UploadedAt
            }).ToList() ?? [],
        };

        await commentRepository.InsertAsync(comment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CommentModel>.Success(commentModel);
    }

    public async Task<Result<CommentModel>> AddReplyAsync(CreateCommentRequest request,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<CommentModel>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated to create replies"));
        }

        var currentUser = await userManager.FindByIdAsync(currentUserId);
        if (currentUser == null)
        {
            return Result<CommentModel>.Failure(new Error("User.NotFound", "User not found"));
        }

        var parentComment =
            await commentRepository.GetByIdAsync(request.ParentCommentId, cancellationToken);
        if (parentComment == null)
        {
            return Result<CommentModel>.Failure(new Error("Comment.NotFound",
                "Parent comment not found"));
        }

        var reply = new Comment
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = currentUserId,
            ParentCommentId = request.ParentCommentId,
            Files = []
        };

        if (request.Files != null && request.Files.Any())
        {
            var uploadResult = await fileUploaderService.UploadFilesAsync(request.Files,
                FileType.Comment, cancellationToken);

            if (uploadResult.IsSuccess && uploadResult.Value?.SuccessfulUploads != null)
            {
                reply.Files = uploadResult.Value.SuccessfulUploads.Select(fileResult =>
                    new ApplicationFile
                    {
                        Id = Guid.NewGuid(),
                        Filename = fileResult.OriginalFileName,
                        Extension = Path.GetExtension(fileResult.OriginalFileName),
                        Size = fileResult.FileSize,
                        FilePath = fileResult.FilePath,
                        Type = fileResult.Type,
                        UserId = currentUserId,
                        UploadedAt = fileResult.UploadedAt
                    }).ToList();
            }
        }

        var replyModel = new CommentModel
        {
            Id = reply.Id,
            Content = reply.Content,
            CreatedAt = reply.CreatedAt,
            UserId = reply.UserId,
            AuthorFirstName = currentUser.FirstName,
            AuthorLastName = currentUser.LastName,
            AuthorUserName = currentUser.UserName ?? string.Empty,
            TimeElapsed = Helpers.GetTimeElapsedString(DateTimeOffset.UtcNow),
            ParentCommentId = request.ParentCommentId,
            Files = []
        };

        await commentRepository.InsertAsync(reply, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CommentModel>.Success(replyModel);
    }

    public async Task<Result<CommentModel>> UpdateCommentAsync(Guid commentId, string content,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<CommentModel>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated to update comments"));
        }

        var comment = await commentRepository.GetByIdAsync(commentId, cancellationToken);
        if (comment == null)
        {
            return Result<CommentModel>.Failure(new Error("Comment.NotFound", "Comment not found"));
        }

        if (comment.UserId != currentUserId)
        {
            return Result<CommentModel>.Failure(new Error("Comment.Unauthorized",
                "You are not authorized to update this comment"));
        }

        var currentUser = await userManager.FindByIdAsync(currentUserId);
        if (currentUser == null)
        {
            return Result<CommentModel>.Failure(new Error("User.NotFound", "User not found"));
        }

        comment.Content = content;
        comment.UpdatedAt = DateTimeOffset.UtcNow;

        await commentRepository.UpdateAsync(comment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var commentModel = new CommentModel
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            UserId = comment.UserId,
            AuthorFirstName = currentUser.FirstName,
            AuthorLastName = currentUser.LastName,
            AuthorUserName = currentUser.UserName ?? string.Empty,
            TimeElapsed = Helpers.GetTimeElapsedString(DateTimeOffset.UtcNow),
            ParentCommentId = comment.ParentCommentId,
            Files = comment.Files?.Select(file => new AttachedFileModel
            {
                Id = file.Id,
                FileName = file.Filename,
                FilePath = file.FilePath,
                Size = file.Size,
                Type = file.Type
            }).ToList() ?? []
        };

        return Result<CommentModel>.Success(commentModel);
    }

    public async Task<Result<bool>> DeleteCommentAsync(Guid commentId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<bool>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated to delete comments"));
        }

        var comment = await commentRepository.GetByIdAsync(commentId, cancellationToken);
        if (comment == null)
        {
            return Result<bool>.Failure(new Error("Comment.NotFound", "Comment not found"));
        }

        if (comment.UserId != currentUserId)
        {
            return Result<bool>.Failure(new Error("Comment.Unauthorized",
                "You are not authorized to delete this comment"));
        }

        await commentRepository.DeleteAsync(comment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success();
    }
}