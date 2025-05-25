using Kite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class FriendRequestController(IFriendRequestService friendRequestService) : BaseApiController
{
    [HttpPost("send-friend-request")]
    public async Task<IActionResult> SendFriendRequest(string targetId)
        => HandleResult(await friendRequestService.SendFriendRequestAsync(targetId));

    [HttpPut("accept-friend-request/{requestId:guid}")]
    public async Task<IActionResult> AcceptFriendRequest(Guid requestId)
        => HandleResult(await friendRequestService.AcceptFriendRequestAsync(requestId));

    [HttpPut("reject-friend-request/{requestId:guid}")]
    public async Task<IActionResult> RejectFriendRequest(Guid requestId)
        => HandleResult(await friendRequestService.RejectFriendRequestAsync(requestId));

    [HttpPut("withdraw-friend-request/{requestId:guid}")]
    public async Task<IActionResult> WithdrawFriendRequest(Guid requestId)
        => HandleResult(await friendRequestService.WithdrawFriendRequestAsync(requestId));

    [HttpGet("get-received-friend-requests")]
    public async Task<IActionResult> GetPendingReceivedRequests()
        => HandleResult(await friendRequestService.GetPendingReceivedRequestsAsync());

    [HttpGet("get-sent-friend-requests")]
    public async Task<IActionResult> GetPendingSentRequests()
        => HandleResult(await friendRequestService.GetPendingSentRequestsAsync());
}