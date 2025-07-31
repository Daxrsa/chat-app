using Microsoft.AspNetCore.Http;

namespace Kite.Application.Models;

public class CreateCommentRequest
{
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string>? MentionedUsers { get; set; } = new();
    public IFormFileCollection? Files { get; set; } 
}