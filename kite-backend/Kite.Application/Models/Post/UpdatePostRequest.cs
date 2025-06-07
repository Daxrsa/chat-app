using Kite.Domain.Enums;

namespace Kite.Application.Models.Post;

public class UpdatePostRequest
{
    public string? Title { get; set; }
    public string? Body { get; set; }
    public PostVisibility? Visibility { get; set; }
    public List<string>? Hashtags { get; set; }
    public List<string>? MentionedUsers { get; set; }
    public List<Guid>? AttachedFileIds { get; set; }
}