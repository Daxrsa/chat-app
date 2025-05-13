using Kite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class FriendshipController(IFriendshipService friendshipService) : BaseApiController
{
    [HttpPost("send-friend-request/{userId}")]
    public async Task<IActionResult> SendFriendRequest(string targetId)
        => HandleResult(await friendshipService.SendFriendRequestAsync(targetId));

    [HttpPut("accept-friend-request/{requestId:guid}")]
    public async Task<IActionResult> AcceptFriendRequest(Guid requestId)
        => HandleResult(await friendshipService.AcceptFriendRequestAsync(requestId));

    [HttpPut("reject-friend-request/{requestId:guid}")]
    public async Task<IActionResult> RejectFriendRequest(Guid requestId)
        => HandleResult(await friendshipService.RejectFriendRequestAsync(requestId));

    [HttpPut("withdraw-friend-request/{requestId:guid}")]
    public async Task<IActionResult> WithdrawFriendRequest(Guid requestId)
        => HandleResult(await friendshipService.WithdrawFriendRequestAsync(requestId));

    [HttpGet("get-received-friend-requests")]
    public async Task<IActionResult> GetPendingReceivedRequests()
        => HandleResult(await friendshipService.GetPendingReceivedRequestsAsync());

    [HttpGet("get-sent-friend-requests")]
    public async Task<IActionResult> GetPendingSentRequests()
        => HandleResult(await friendshipService.GetPendingSentRequestsAsync());

    [HttpGet("get-all-friends")]
    public async Task<IActionResult> GetFriends()
        => HandleResult(await friendshipService.GetFriendsAsync());
}