using Kite.Application.Interfaces;
using Kite.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Web.Controllers;

public class CommentController(ICommentService commentService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateComment([FromForm] CreateCommentRequest request,
        CancellationToken cancellationToken = default)
        => HandleResult(await commentService.AddCommentAsync(request, cancellationToken));

    [HttpPost("reply")]
    public async Task<IActionResult> CreateReply([FromForm] CreateCommentRequest request,
        CancellationToken cancellationToken = default)
        => HandleResult(await commentService.AddReplyAsync(request, cancellationToken));

    [HttpPut("{commentId:guid}")]
    public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] string content,
        CancellationToken cancellationToken = default)
        => HandleResult(
            await commentService.UpdateCommentAsync(commentId, content, cancellationToken));

    [HttpDelete("{commentId:guid}")]
    public async Task<IActionResult> DeleteComment(Guid commentId,
        CancellationToken cancellationToken = default)
        => HandleResult(await commentService.DeleteCommentAsync(commentId, cancellationToken));
    
    [HttpGet("{commentId:guid}/reactions/count")]
    public async Task<IActionResult> GetCommentTotalReactions(Guid commentId,
        CancellationToken cancellationToken = default)
        => HandleResult(await commentService.GetCommentTotalReactionsAsync(commentId, cancellationToken));
}