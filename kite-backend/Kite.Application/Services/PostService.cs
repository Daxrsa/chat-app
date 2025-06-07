using Kite.Application.Interfaces;
using Kite.Application.Models;
using Kite.Application.Models.Post;
using Kite.Domain.Common;
using Kite.Domain.Entities;
using Kite.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kite.Application.Services;

public class PostService(
    IUserAccessor userAccessor,
    UserManager<ApplicationUser> userManager,
    IPostRepository postRepository) : IPostService
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

            var post = new Post
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Body = request.Body,
                CreatedAt = DateTimeOffset.UtcNow,
                UserId = currentUserId,
                User = user
            };

            // Add to repository
            await postRepository.InsertAsync(post, cancellationToken);

            // Convert to model and return
            var postModel = new PostModel
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                CreatedAt = post.CreatedAt,
                UserId = post.UserId,
                AuthorFirstName = user.FirstName,
                AuthorLastName = user.LastName,
                AuthorUsername = user.UserName,
                AuthorProfilePicture = null, // TODO: Get from user's files
                Visibility = request.Visibility,
                Hashtags = request.Hashtags,
                MentionedUsers = request.MentionedUsers,
                LikeCount = 0,
                CommentCount = 0,
                ShareCount = 0,
                IsLikedByCurrentUser = false,
                IsEdited = false,
                IsHidden = false,
                AttachedFiles = ""
            };

            return Result<PostModel>.Success(postModel);
        }
        catch (Exception ex)
        {
            return Result<PostModel>.Failure(new Error("Post.CreateFailed",
                $"Failed to create post: {ex.Message}"));
        }
    }

    public Task<Result<PostModel>> GetPostByIdAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<PostModel>> UpdatePostAsync(Guid postId, UpdatePostRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeletePostAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}