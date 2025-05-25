using Kite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class FriendController(IFriendshipService friendshipService) : BaseApiController
{
    [HttpGet("get-all-friends")]
    public async Task<IActionResult> GetFriends()
        => HandleResult(await friendshipService.GetFriendsAsync());
    
    [HttpDelete("remove-friend")]
    public async Task<IActionResult> RemoveFriend(string targetId)
        => HandleResult(await friendshipService.RemoveFriendAsync(targetId));
}