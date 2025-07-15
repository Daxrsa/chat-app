using Kite.Application.Interfaces;
using Kite.Application.Models.Post;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class PostController(IPostService postService) : BaseApiController
{
    [HttpPost("create-post")]
    public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest model)
        => HandleResult(await postService.CreatePostAsync(model));

    [HttpGet("user-posts")]
    public async Task<IActionResult> GetCurrentUserPosts()
        => HandleResult(await postService.GetPostsForCurrentUserAsync());

    [HttpGet("user/{userId}/posts")]
    public async Task<IActionResult> GetUserPosts(string userId)
        => HandleResult(await postService.GetPostsForUserAsync(userId));

    [HttpGet("{postId}")]
    public async Task<IActionResult> GetSinglePost(Guid postId)
        => HandleResult(await postService.GetSinglePostAsync(postId));

    [HttpPut("{postId}")]
    public async Task<IActionResult> UpdatePost(Guid postId, [FromForm] UpdatePostRequest model)
        => HandleResult(await postService.UpdatePostAsync(postId, model));

    [HttpDelete("{postId}")]
    public async Task<IActionResult> DeletePost(Guid postId)
        => HandleResult(await postService.DeletePostAsync(postId));
}