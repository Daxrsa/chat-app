using Kite.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Kite.Application.Models.Post;

public class CreatePostRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
    public List<string> Hashtags { get; set; } = new();
    public List<string> MentionedUsers { get; set; } = new();
    public IFormFileCollection AttachedFiles { get; set; }
}