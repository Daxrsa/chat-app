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

        var uploadResult = await fileUploaderService.UploadFilesAsync(request.Files,
            FileType.Comment, cancellationToken);

        var applicationFiles = uploadResult.Value.SuccessfulUploads.Select(fileResult =>
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
        
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = currentUserId,
            Files = applicationFiles
        };
        
        var commentModel = new CommentModel
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UserId = comment.UserId,
            AuthorFirstName = currentUser.FirstName,
            AuthorLastName = currentUser.LastName,
            AuthorUserName = currentUser.UserName ?? string.Empty,
            TimeElapsed = Helpers.GetTimeElapsedString(DateTimeOffset.UtcNow),
            Files = applicationFiles.Select(file => new AttachedFileModel
            {
                Id = file.Id,
                FileName = file.Filename,
                FilePath = file.FilePath,
                Size = file.Size,
                Type = file.Type
            }).ToList(),
        };
        
        await commentRepository.InsertAsync(comment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CommentModel>.Success(commentModel);
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

    public async Task<Result<bool>> DeleteCommentAsync(Guid commentId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}