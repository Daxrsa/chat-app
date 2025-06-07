using Kite.Domain.Enums;

namespace Kite.Application.Models.Post;

public class CreatePostRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
    public List<string> Hashtags { get; set; } = new();
    public List<string> MentionedUsers { get; set; } = new();
    public List<Guid> AttachedFileIds { get; set; } = new();
}