using Kite.Application.Interfaces;
using Kite.Application.Models.Post;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class PostController(IPostService postService) : BaseApiController
{
    [HttpPost("create-post")]
    public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest model)
        => HandleResult(await postService.CreatePostAsync(model));

    [HttpGet("{postId:guid}")]
    public async Task<IActionResult> GetSinglePost(Guid postId)
        => HandleResult(await postService.GetSinglePostAsync(postId));

    [HttpPut("{postId:guid}")]
    public async Task<IActionResult> UpdatePost(Guid postId, [FromForm] UpdatePostRequest model)
        => HandleResult(await postService.UpdatePostAsync(postId, model));

    [HttpDelete("{postId:guid}")]
    public async Task<IActionResult> DeletePost(Guid postId)
        => HandleResult(await postService.DeletePostAsync(postId));

    [HttpGet("{postId}/reactions/count")]
    public async Task<IActionResult> GetPostTotalReactions(Guid postId,
        CancellationToken cancellationToken = default)
        => HandleResult(await postService.GetPostTotalReactionsAsync(postId, cancellationToken));
}