using Kite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

[Route("api/[controller]/{conversationId:guid}")]
public class ConversationParticipantController(
    IConversationParticipantService conversationParticipantService) : BaseApiController
{
    [HttpPost("participants/{userId}")]
    public async Task<IActionResult> AddParticipant(Guid conversationId, string userId,
        CancellationToken cancellationToken = default)
        => HandleResult(
            await conversationParticipantService.AddParticipantAsync(conversationId, userId,
                cancellationToken));

    [HttpDelete("participants/{userId}")]
    public async Task<IActionResult> RemoveParticipant(Guid conversationId, string userId,
        CancellationToken cancellationToken = default)
        => HandleResult(
            await conversationParticipantService.RemoveParticipantAsync(conversationId, userId,
                cancellationToken));

    [HttpGet("participants")]
    public async Task<IActionResult> GetParticipants(Guid conversationId,
        CancellationToken cancellationToken = default)
        => HandleResult(
            await conversationParticipantService.GetParticipantsAsync(conversationId,
                cancellationToken));

    [HttpGet("non-participants")]
    public async Task<IActionResult> GetNonParticipants(Guid conversationId,
        CancellationToken cancellationToken = default)
        => HandleResult(
            await conversationParticipantService.GetNonParticipantsAsync(conversationId,
                cancellationToken));
    
    [HttpPost("promote-moderator/{userId}")]
    public async Task<IActionResult> PromoteToModerator(Guid conversationId, string userId,
        CancellationToken cancellationToken = default)
        => HandleResult(
            await conversationParticipantService.PromoteToModeratorAsync(conversationId, userId,
                cancellationToken));

    [HttpDelete("demote-moderator/{userId}")]
    public async Task<IActionResult> DemoteModerator(Guid conversationId, string userId,
        CancellationToken cancellationToken = default)
        => HandleResult(
            await conversationParticipantService.DemoteModeratorAsync(conversationId, userId,
                cancellationToken));

    [HttpPost("transfer-ownership/{newOwnerUserId}")]
    public async Task<IActionResult> TransferOwnership(Guid conversationId, string newOwnerUserId,
        CancellationToken cancellationToken = default)
        => HandleResult(await conversationParticipantService.TransferOwnershipAsync(conversationId,
            newOwnerUserId, cancellationToken));
}