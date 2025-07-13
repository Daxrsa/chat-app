using Kite.Application.Interfaces;
using Kite.Application.Models.Post;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class PostController(IPostService postService) : BaseApiController
{
    [HttpPost("create-post")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest model)
        => HandleResult(await postService.CreatePostAsync(model));
}