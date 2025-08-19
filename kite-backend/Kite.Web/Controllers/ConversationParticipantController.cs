using Kite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class ConversationParticipantController(IConversationParticipantService conversationParticipantService) : BaseApiController
{
    [HttpPost("{conversationId}/participants/{userId}")]
    public async Task<IActionResult> AddParticipant(Guid conversationId, string userId, CancellationToken cancellationToken = default)
        => HandleResult(await conversationParticipantService.AddParticipantAsync(conversationId, userId, cancellationToken));

    [HttpDelete("{conversationId}/participants/{userId}")]
    public async Task<IActionResult> RemoveParticipant(Guid conversationId, string userId, CancellationToken cancellationToken = default)
        => HandleResult(await conversationParticipantService.RemoveParticipantAsync(conversationId, userId, cancellationToken));

    [HttpGet("{conversationId}/participants")]
    public async Task<IActionResult> GetParticipants(Guid conversationId, CancellationToken cancellationToken = default)
        => HandleResult(await conversationParticipantService.GetParticipantsAsync(conversationId, cancellationToken));

    [HttpGet("{conversationId}/non-participants")]
    public async Task<IActionResult> GetNonParticipants(Guid conversationId, CancellationToken cancellationToken = default)
        => HandleResult(await conversationParticipantService.GetNonParticipantsAsync(conversationId, cancellationToken));
}