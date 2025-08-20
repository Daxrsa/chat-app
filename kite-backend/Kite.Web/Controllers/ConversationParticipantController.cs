using Kite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

[Route("api/[controller]/{conversationId:guid}")]
public class ConversationParticipantController(IConversationParticipantService conversationParticipantService) : BaseApiController
{
    [HttpPost("participants/{userId}")]
    public async Task<IActionResult> AddParticipant(Guid conversationId, string userId, CancellationToken cancellationToken = default)
        => HandleResult(await conversationParticipantService.AddParticipantAsync(conversationId, userId, cancellationToken));

    [HttpDelete("participants/{userId}")]
    public async Task<IActionResult> RemoveParticipant(Guid conversationId, string userId, CancellationToken cancellationToken = default)
        => HandleResult(await conversationParticipantService.RemoveParticipantAsync(conversationId, userId, cancellationToken));

    [HttpGet("participants")]
    public async Task<IActionResult> GetParticipants(Guid conversationId, CancellationToken cancellationToken = default)
        => HandleResult(await conversationParticipantService.GetParticipantsAsync(conversationId, cancellationToken));

    [HttpGet("non-participants")]
    public async Task<IActionResult> GetNonParticipants(Guid conversationId, CancellationToken cancellationToken = default)
        => HandleResult(await conversationParticipantService.GetNonParticipantsAsync(conversationId, cancellationToken));
}