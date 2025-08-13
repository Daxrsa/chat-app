using Kite.Application.Interfaces;
using Kite.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class ReactionController(IReactionService reactionService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> AddOrUpdateReaction([FromBody] AddReactionRequest request,
        CancellationToken cancellationToken = default)
        => HandleResult(await reactionService.AddOrUpdateReactionAsync(request.EntityType,
            request.EntityId,
            request.ReactionType, cancellationToken));

    [HttpGet]
    public async Task<IActionResult> RemoveReaction([FromQuery] Guid reactionId,
        CancellationToken cancellationToken = default)
        => HandleResult(await reactionService.RemoveReaction(reactionId, cancellationToken));
}