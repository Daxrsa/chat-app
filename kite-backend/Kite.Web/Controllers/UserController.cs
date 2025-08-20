using Kite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class UserController(IUserService userService) : BaseApiController
{
    [HttpGet("user-conversations")]
    public async Task<IActionResult> GetUserConversations()
        => HandleResult(await userService.GetUserConversationsAsync());
    
    [HttpGet("user-posts")]
    public async Task<IActionResult> GetCurrentUserPosts()
        => HandleResult(await userService.GetPostsForCurrentUserAsync());

    [HttpGet("user/{userId}/posts")]
    public async Task<IActionResult> GetUserPosts(string userId)
        => HandleResult(await userService.GetPostsForUserAsync(userId));
}