using AutoMapper;
using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Application.Models.Post;
using Kite.Application.Utilities;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Kite.Domain.Enums;
using Kite.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kite.Application.Services;

public class PostService(
    IUserAccessor userAccessor,
    UserManager<ApplicationUser> userManager,
    IPostRepository postRepository,
    IApplicationFileRepository applicationFileRepository,
    IFileUploaderService fileUploaderService,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IPostService
{
    public async Task<Result<PostModel>> CreatePostAsync(CreatePostRequest request,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<PostModel>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated to create posts"));
        }

        var currentUser = await userManager.FindByIdAsync(currentUserId);
        if (currentUser == null)
        {
            return Result<PostModel>.Failure(new Error("User.NotFound", "User not found"));
        }

        var uploadResult = await fileUploaderService.UploadFilesAsync(request.Files,
            FileType.Post, cancellationToken);

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

        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Body = request.Body,
            CreatedAt = DateTimeOffset.UtcNow,
            Hashtags = request.Hashtags,
            UserId = currentUserId,
            Files = applicationFiles
        };
        
        if (request.MentionedUsers?.Count != 0)
        {
            var users = new List<ApplicationUser>();
            foreach (var userId in request.MentionedUsers)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    users.Add(user);
                }
            }
            post.MentionedUsers = users;
        }

        var authorProfilePicture =
            await applicationFileRepository.GetLatestUserFileByTypeAsync(currentUserId,
                FileType.Post, cancellationToken);

        var postModel = new PostModel
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            CreatedAt = post.CreatedAt,
            UserId = post.UserId,
            AuthorFirstName = currentUser.FirstName,
            AuthorLastName = currentUser.LastName,
            AuthorUserName = currentUser.UserName ?? string.Empty,
            AuthorProfilePicture = authorProfilePicture?.FilePath,
            Visibility = request.Visibility,
            Hashtags = request.Hashtags,
            MentionedUsers = request.MentionedUsers,
            ReactionCount = 0,
            CommentCount = 0,
            ShareCount = 0,
            IsReactedByCurrentUser = false,
            IsEdited = false,
            IsHidden = false,
            Files = post.Files?.Select(file => new AttachedFileModel
            {
                Id = file.Id,
                FileName = file.Filename,
                Extension = file.Extension,
                Size = file.Size,
                FilePath = file.FilePath,
                Type = file.Type,
                UserId = file.UserId,
                UploadedAt = file.UploadedAt
            }).ToList() ?? new List<AttachedFileModel>(),
            TimeElapsed = Helpers.GetTimeElapsedString(DateTimeOffset.UtcNow)
        };

        await postRepository.InsertAsync(post, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PostModel>.Success(postModel);
    }

    public async Task<Result<List<PostModel>>> GetPostsForCurrentUserAsync(
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();

        var posts = await postRepository.GetPostsForUserAsync(currentUserId, cancellationToken);
        if (posts is null)
        {
            return Result<List<PostModel>>.Failure(new Error("Posts.NotFound", "No posts found"));
        }

        var postModels = mapper.Map<List<PostModel>>(posts);

        return Result<List<PostModel>>.Success(postModels);
    }

    public async Task<Result<List<PostModel>>> GetPostsForUserAsync(string userId,
        CancellationToken cancellationToken = default)
    {
        var posts = await postRepository.GetPostsForUserAsync(userId, cancellationToken);
        if (posts is null)
        {
            return Result<List<PostModel>>.Failure(new Error("Posts.NotFound", "No posts found"));
        }

        var postModels = mapper.Map<List<PostModel>>(posts);

        return Result<List<PostModel>>.Success(postModels);
    }

    public async Task<Result<PostModel>> GetSinglePostAsync(Guid postId,
        CancellationToken cancellationToken = default)
    {
        var post = await postRepository.GetByIdAsync(postId, cancellationToken);
        if (post is null)
        {
            return Result<PostModel>.Failure(new Error("Post.NotFound", "Post not found"));
        }

        var user = await userManager.FindByIdAsync(post.UserId);

        var authorProfilePicture =
            await applicationFileRepository.GetLatestUserFileByTypeAsync(post.UserId, FileType.ProfilePicture,
                cancellationToken);

        var postFiles = await applicationFileRepository.GetFilesByPostIdAsync(postId, cancellationToken);

        var postModel = mapper.Map<PostModel>(post);
    
        if (user != null)
        {
            postModel.AuthorFirstName = user.FirstName;
            postModel.AuthorLastName = user.LastName;
            postModel.AuthorUserName = user.UserName ?? string.Empty;
        }

        postModel.AuthorProfilePicture = authorProfilePicture != null 
            ? mapper.Map<AttachedFileModel>(authorProfilePicture).FilePath 
            : null;
        postModel.Files = mapper.Map<List<AttachedFileModel>>(postFiles) ?? new List<AttachedFileModel>();

        return Result<PostModel>.Success(postModel);
    }

    public async Task<Result<PostModel>> UpdatePostAsync(Guid postId, UpdatePostRequest request,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<PostModel>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated to update posts"));
        }

        var post = await postRepository.GetByIdAsync(postId, cancellationToken);
        if (post == null)
        {
            return Result<PostModel>.Failure(new Error("Post.NotFound", "Post not found"));
        }

        if (post.UserId != currentUserId)
        {
            return Result<PostModel>.Failure(new Error("Post.Unauthorized",
                "You are not authorized to update this post"));
        }
        
        if (request.MentionedUsers?.Count != 0)
        {
            var users = new List<ApplicationUser>();
            foreach (var userId in request.MentionedUsers)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    users.Add(user);
                }
            }
            post.MentionedUsers = users;
        }
        
        post.Title = request.Title;
        post.Body = request.Body;
        post.Hashtags = request.Hashtags;
        post.Visibility = request.Visibility ?? post.Visibility;
        post.IsEdited = true;
        post.UpdatedAt = DateTimeOffset.UtcNow;
        
        if (request.Files != null)
        {
            await fileUploaderService.UploadFilesAsync(request.Files, FileType.Post, cancellationToken);
        }

        await postRepository.UpdateAsync(post, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedPost = await postRepository.GetByIdAsync(postId, cancellationToken);
        var postModel = mapper.Map<PostModel>(updatedPost);

        return Result<PostModel>.Success(postModel);
    }

    public async Task<Result<bool>> DeletePostAsync(Guid postId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Result<bool>.Failure(new Error("Auth.Unauthorized",
                "User must be authenticated to delete posts"));
        }

        var post = await postRepository.GetByIdAsync(postId, cancellationToken);
        if (post.UserId != currentUserId)
        {
            return Result<bool>.Failure(new Error("Post.Unauthorized",
                "You are not authorized to delete this post"));
        }

        await postRepository.DeleteAsync(post, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success();
    }
}