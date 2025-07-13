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
        try
        {
            var currentUserId = userAccessor.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Result<PostModel>.Failure(new Error("Auth.Unauthorized",
                    "User must be authenticated to create posts"));
            }

            var user = await userManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                return Result<PostModel>.Failure(new Error("User.NotFound", "User not found"));
            }

            // var files = await fileUploaderService.UploadFilesAsync(request.AttachedFiles,
            //     FileType.Post, cancellationToken);

            var post = new Post
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Body = request.Body,
                CreatedAt = DateTimeOffset.UtcNow,
                UserId = currentUserId,
                TimeElapsed = Helpers.GetTimeElapsedString(DateTimeOffset.UtcNow),
                // Files = files,
            };

            var authorProfilePicture = await applicationFileRepository.GetLatestUserFileByTypeAsync(currentUserId, FileType.Post ,cancellationToken);
            
            var postModel = new PostModel
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                CreatedAt = post.CreatedAt,
                UserId = post.UserId,
                AuthorFirstName = user.FirstName,
                AuthorLastName = user.LastName,
                AuthorUserName = user.UserName,
                AuthorProfilePicture = authorProfilePicture.FilePath,
                Visibility = request.Visibility,
                Hashtags = request.Hashtags,
                MentionedUsers = request.MentionedUsers,
                LikeCount = 0,
                CommentCount = 0,
                ShareCount = 0,
                IsLikedByCurrentUser = false,
                IsEdited = false,
                IsHidden = false,
                // AttachedFiles = post.Files,
                TimeElapsed = Helpers.GetTimeElapsedString(post.CreatedAt)
            };
            
            await postRepository.InsertAsync(post, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<PostModel>.Success(postModel);
        }
        catch (Exception ex)
        {
            return Result<PostModel>.Failure(new Error("Post.CreationFailed",
                $"Failed to create post: {ex.Message}"));
        }
    }
    
    public async Task<Result<List<PostModel>>> GetPostsForCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = userAccessor.GetCurrentUserId();
        
        var posts = await postRepository.GetPostsForUserAsync(currentUserId, cancellationToken);
        if (posts == null)
        {
            return Result<List<PostModel>>.Failure(new Error("Posts.NotFound", "No posts found"));
        }
        
        var postModels = mapper.Map<List<PostModel>>(posts);
        
        return Result<List<PostModel>>.Success(postModels);
    }

    public async Task<Result<List<PostModel>>> GetPostsForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var posts = await postRepository.GetPostsForUserAsync(userId, cancellationToken);
        if (posts == null)
        {
            return Result<List<PostModel>>.Failure(new Error("Posts.NotFound", "No posts found"));
        }
        
        var postModels = mapper.Map<List<PostModel>>(posts);
        
        return Result<List<PostModel>>.Success(postModels);
    }

    public async Task<Result<PostModel>> GetSinglePostAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        var post = await postRepository.GetByIdAsync(postId, cancellationToken);
        if (post == null)
        {
            return Result<PostModel>.Failure(new Error("Post.NotFound", "Post not found"));
        }

        var postModel = mapper.Map<PostModel>(post);

        return Result<PostModel>.Success(postModel);
    }

    public async Task<Result<PostModel>> UpdatePostAsync(Guid postId, UpdatePostRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<bool>> DeletePostAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}