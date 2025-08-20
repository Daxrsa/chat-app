using Kite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class ConversationController(IConversationService conversationService) : BaseApiController
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateConversation([FromBody] List<string> participantIds)
        => HandleResult(await conversationService.CreateConversationAsync(participantIds));

    [HttpGet("{conversationId:guid}/messages")]
    public async Task<IActionResult> GetConversationMessages(Guid conversationId)
        => HandleResult(await conversationService.GetConversationMessagesAsync(conversationId));

    [HttpDelete("{conversationId:guid}")]
    public async Task<IActionResult> DeleteConversation(Guid conversationId)
        => HandleResult(await conversationService.DeleteConversationAsync(conversationId));
}